using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace WhatMunch_MAUI.Services
{
    public interface IPermissionsService
    {
        Task<bool> CheckAndRequestLocationPermissionAsync();
    }

    public class PermissionsService(ILogger<PermissionsService> logger) : IPermissionsService
    {
        [ExcludeFromCodeCoverage]
        public Func<Task<PermissionStatus>> CheckStatusAsyncMethod { get; set; } = 
            () => Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        [ExcludeFromCodeCoverage]
        public Func<Task<PermissionStatus>> RequestAsyncMethod { get; set; } =
            () => Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        public async Task<bool> CheckAndRequestLocationPermissionAsync()
        {
            try
            {
                var status = await CheckStatusAsyncMethod.Invoke();

                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsyncMethod.Invoke();
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while checking or requesting location permission.");
                return false;
            }
        }
    }
}
