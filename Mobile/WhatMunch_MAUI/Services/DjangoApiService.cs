using Microsoft.Extensions.Logging;
using System.Net;
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
        IAccountService accountService,
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
                var client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                await tokenService.UpdateHeaders(client);
                var response = await client.GetAsync("get-places-key/");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var refreshResult = await accountService.RefreshAccessTokenAsync();
                    if (refreshResult.IsSuccess)
                    {
                        client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                        await tokenService.UpdateHeaders(client);
                        response = await client.GetAsync("get-places-key/");
                    }
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                _cachedKey = result?["googleMapsApiKey"] ?? throw new Exception("API key missing in response");

                return _cachedKey;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP request failed: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                throw;
            }
        }
    }
}
