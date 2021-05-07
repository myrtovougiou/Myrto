using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeChallenge.WebApp.Services
{
    public class BaseService
    {
        private HttpClientHandler CreateHttpClientHandler()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            return clientHandler;
        }

        protected HttpClient CreateHttpClient()
        {
            var client = new HttpClient(CreateHttpClientHandler());
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
