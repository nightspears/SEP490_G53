using System.Net.Http.Headers;

namespace TCViettelFC_Client.Services
{
    public class BaseService
    {
        protected readonly HttpClient _httpClient;

        public BaseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<HttpResponseMessage> GetAsync(string endpoint, string token = null)
        {
            // Set Authorization token if provided
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync(endpoint);
            return response;
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string token = null)
        {
            // Set Authorization token if provided
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            return response;
        }
    }
}
