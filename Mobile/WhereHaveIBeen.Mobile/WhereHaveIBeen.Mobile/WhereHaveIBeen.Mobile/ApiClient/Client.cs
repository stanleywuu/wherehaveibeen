using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WhereHaveIBeen.Mobile.ApiClient
{
    public static class Client
    {
        private static readonly Lazy<HttpClient> lazy = new Lazy<HttpClient>(() => new HttpClient()
        {
            BaseAddress = new Uri("https://wherehaveibeen.azurewebsites.net/")
        });

        public static HttpClient Instance => lazy.Value;
    }
}
