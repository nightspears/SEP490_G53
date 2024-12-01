using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TCViettelFC_Client.ApiServices
{
    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string? token;
        public ApiHelper(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
            token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        public async Task<T> GetApiResponseAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync("https://localhost:5000/api/" + endpoint);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), options);
            }
            return default;
        }
        public async Task<string> CreateApiResponseAsync<T>(string endpoint, T data)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:5000/api/" + endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return "Add successfully";
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Unauthorized";
            }
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> DeleteApiResponseAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync("https://localhost:5000/api/" + endpoint);
            if (response.IsSuccessStatusCode)
            {
                return "Delete successfully";
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Unauthorized";
            }
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> UpdateApiResponseAsync<T>(string endpoint, T data)
        {
            var response = await _httpClient.PutAsJsonAsync("https://localhost:5000/api/" + endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                return "Update successfully" + await response.Content.ReadAsStringAsync();
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Unauthorized";
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
