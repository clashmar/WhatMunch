using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using WhatMunch_MAUI.Utility.Exceptions;

namespace WhatMunch_MAUI.Services
{
    public interface ILocationService
    {
        Task<Location> GetLocationWithTimeoutAsync();
        Task<Location> GetLastSearchLocation();
    }
    public class LocationService(
        IGeolocation geolocation, 
        IPermissionsService permissionsService, 
        ILogger<LocationService> logger) : ILocationService
    {
        private Location? lastSearchLocation;

        [ExcludeFromCodeCoverage]
        public Func<IGeolocation, GeolocationRequest, Task<Location?>> GetLocationAsyncMethod { get; set; } 
            = (obj, geo) => obj.GetLocationAsync(geo);

        public async Task<Location> GetLocationWithTimeoutAsync()
        {
            try
            {
                if (await permissionsService.CheckAndRequestLocationPermissionAsync())
                {
                    var location = await geolocation.GetLastKnownLocationAsync() ??
                        await GetLocationAsyncMethod.Invoke(geolocation, new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.High,
                            Timeout = TimeSpan.FromSeconds(10)
                        }) ?? throw new LocationException("Location services are disabled or unavailable.");

                    lastSearchLocation = location;
                    return location;
                }
                else
                {
                    logger.LogError("Location permissions are disabled or unavailable");
                    throw new LocationException("Location permissions are disabled or unavailable");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while getting geolocation");
                throw;
            }
        }

        public async Task<Location> GetLastSearchLocation()
        {
            return lastSearchLocation is not null ? lastSearchLocation : await GetLocationWithTimeoutAsync();
        }
    }
}
