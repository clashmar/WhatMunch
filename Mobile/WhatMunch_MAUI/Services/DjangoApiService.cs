using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;

namespace WhatMunch_MAUI.Services
{
    public interface IDjangoApiService
    {
        Task<string> GetGoogleMapsApiKeyAsync();
    }
    public class DjangoApiService(
        IHttpClientFactory clientFactory, 
        ILogger<DjangoApiService> logger,
        ITokenService tokenService) : IDjangoApiService
    {
        private string? _cachedKey;

        // TODO: Unit tests GetGoogleMapsApiKeyAsync
        public async Task<string> GetGoogleMapsApiKeyAsync()
        {
            if (!string.IsNullOrEmpty(_cachedKey))
                return _cachedKey;

            try
            {
                using var client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                await tokenService.UpdateHeaders(client);

                var response = await client.GetAsync("get-places-key/");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                _cachedKey = result?["googleMapsApiKey"] ?? throw new Exception("API key missing in response");

                return _cachedKey;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get Google Maps API key from Django.");
                throw;
            }
        }
    }
}
