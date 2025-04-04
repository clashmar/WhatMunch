namespace WhatMunch_MAUI.Services
{
    public interface IPermissionsService
    {
        Task<bool> CheckAndRequestLocationPermissionAsync();
    }

    public class PermissionsService : IPermissionsService
    {
        public async Task<bool> CheckAndRequestLocationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }
    }
}
