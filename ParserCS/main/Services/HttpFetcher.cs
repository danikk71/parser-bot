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
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(html))
                throw new Exception($"Сайт видав пусту сторінку на {url}");
            return html;
        }
    }
}
