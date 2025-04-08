using Microsoft.Extensions.Logging;

namespace WhatMunch_MAUI.Services
{
    public interface ILocationService
    {
        Task<Location> GetLocationWithTimeoutAsync();
    }
    public class LocationService : ILocationService
    {
        private readonly IGeolocation _geolocation;
        private readonly IPermissionsService _permissionsService;
        private readonly ILogger<LocationService> _logger;

        public LocationService(IGeolocation geolocation, IPermissionsService permissionsService, ILogger<LocationService> logger)
        {
            _geolocation = geolocation;
            _permissionsService = permissionsService;
            _logger = logger;
        }

        public async Task<Location> GetLocationWithTimeoutAsync()
        {
            try
            {
                if (await _permissionsService.CheckAndRequestLocationPermissionAsync())
                {
                    var lastLocation = await _geolocation.GetLastKnownLocationAsync();

                    var location = await _geolocation.GetLastKnownLocationAsync() ??
                        await _geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.High,
                            Timeout = TimeSpan.FromSeconds(10)
                        }) ?? throw new InvalidOperationException("Location services are disabled or unavailable.");

                    return location;
                }
                else
                {
                    _logger.LogError("Location permissions are disabled or unavailable");
                    throw new InvalidOperationException("Location permissions are disabled or unavailable");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting geolocation");
                throw;
            }
        }
    }
}
