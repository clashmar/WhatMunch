using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IGooglePlacesService
    {

    }

    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GooglePlacesService> _logger;
        private readonly string _apiKey;

        public GooglePlacesService(
            IConfiguration configuration, 
            IHttpClientFactory clientFactory, 
            ILogger<GooglePlacesService> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
            _apiKey = _configuration["GoogleMapsApiKey"] ?? "";
        }

        public async Task<Result<int>> GetNearbySearchResults()
        {
            try
            {

            }
            catch (Exception)
            {
                _logger.LogError("An unexpected error occurred in GooglePlacesService.");
                return Result<IStockPriceData>.Failure(AppResources.ErrorUnexpected);
            }

            return Result<int>.Success(2);
        }
}
