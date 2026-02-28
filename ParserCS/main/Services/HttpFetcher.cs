using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Interfaces;

namespace main.Services
{
    public class HttpFetcher : IWebFetcher
    {
        private readonly HttpClient _client;

        public HttpFetcher(HttpClient client)
        {
            _client = client;
        }
        public async Task<string> FetchHTMLAsync(string url)
        {
            var response = await _client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Gone ||
                response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return string.Empty;
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
