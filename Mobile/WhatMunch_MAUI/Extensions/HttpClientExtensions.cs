using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpClient UpdateLanguageHeaders(this HttpClient client)
        {
            string language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
            return client;
        }

        // TODO; Unit test ExecuteRequestWithRefreshAsync
        public static async Task<HttpResponseMessage> ExecuteRequestWithRefreshAsync(
        this HttpClient client,
        Func<HttpClient, Task<HttpResponseMessage>> request,
        IAccountService accountService,
        ITokenService tokenService,
        IHttpClientFactory clientFactory)
        {
            await tokenService.UpdateHeaders(client);
            var response = await request(client);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshResult = await accountService.RefreshAccessTokenAsync();
                if (refreshResult.IsSuccess)
                {
                    client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                    await tokenService.UpdateHeaders(client);

                    response = await request(client);
                }
            }
            return response;
        }
    }
}

    
