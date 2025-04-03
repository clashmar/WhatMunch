using Microsoft.Extensions.Logging;

namespace WhatMunch_MAUI.Services
{
    public interface ISearchService
    {

    }

    public class SearchService : ISearchService
    {
        private readonly IShellService _shellService;
        private readonly ILogger<SearchService> _logger;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IConnectivity _connectivity;

        public SearchService(
            IShellService shellService, 
            ILogger<SearchService> logger, 
            IGooglePlacesService googlePlacesService, 
            IConnectivity connectivity)
        {
            _shellService = shellService;
            _logger = logger;
            _googlePlacesService = googlePlacesService;
            _connectivity = connectivity;
        }
    }
}
