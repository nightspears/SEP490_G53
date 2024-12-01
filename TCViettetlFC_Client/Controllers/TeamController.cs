using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCViettetlFC_Client.Models;

namespace TCViettelFC_Client.Controllers
{
    public class TeamController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public TeamController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _baseAddress = configuration["ApiConfig:BaseAddress"]; // Retrieve BaseAddress from appsettings.json
        }
        
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_baseAddress}Players/active");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var players = JsonConvert.DeserializeObject<List<PlayerViewModel>>(content);
                return View(players);
            }
            else
            {
                // Handle error (e.g., log it, show error message)
                return View(new List<PlayerViewModel>());
            }
        }

        public async Task<IActionResult> PlayerDetails(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            var response = await _httpClient.GetAsync($"{_baseAddress}Players/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var player = JsonConvert.DeserializeObject<PlayerViewModel>(content);
                return View(player);
            }
            else
            {
                // Handle error (e.g., player not found)
                return NotFound("Player not found from API.");
            }
        }
    }
}
