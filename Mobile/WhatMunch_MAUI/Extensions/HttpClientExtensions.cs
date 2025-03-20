using System.Globalization;
using System.Net.Http.Headers;

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
    }
}
