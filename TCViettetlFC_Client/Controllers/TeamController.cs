using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCViettetlFC_Client.Models;

namespace TCViettelFC_Client.Controllers
{
    public class TeamController : Controller
    {
        private readonly HttpClient _httpClient;

        public TeamController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:5000/api/Players/ListPlayer");
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
                return BadRequest("ID không thấy.");
            }

            // Call API to get player details
            var response = await _httpClient.GetAsync($"https://localhost:5000/api/Players/GetPlayerById?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var player = JsonConvert.DeserializeObject<PlayerViewModel>(content);
                return View(player);
            }
            else
            {
                // Handle error (e.g., player not found)
                return NotFound("ko thấy người chơi từ api.");
            }
        }
    }
}
