using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class MatchController : Controller
    {
        private readonly HttpClient _httpClient;



        public MatchController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");

        }
        public async Task<IActionResult> Index()
        {
            // Call API to get matches
            var response = await _httpClient.GetAsync("https://localhost:5000/api/Matches/GetMatches");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(content);
                return View(matches);
            }
            else
            {
                // Handle error (e.g., log it, show error message)
                return View(new List<MatchViewModel>());
            }
        }
    }
}
