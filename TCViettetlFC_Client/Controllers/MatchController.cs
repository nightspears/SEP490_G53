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
            List<MatchViewModel> matches = new List<MatchViewModel>(); // Default empty list

            try
            {
                // Attempt to call API to get matches
                var response = await _httpClient.GetAsync("https://localhost:5000/api/Matches/GetMatchesNotStartYetAsync");

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(content);
                }
                else
                {
                    // Handle API failure (non-success status code)
                    ViewData["ErrorMessage"] = "Không thể tải trận. Vui lòng tải lại.";
                }
            }
            catch (HttpRequestException)
            {
                // Handle HTTP request exceptions (e.g., API not reachable)
                ViewData["ErrorMessage"] = "API không hoạt động, hãy kiểm tra kết nối";
            }
            catch (Exception)
            {
                // Handle any other unexpected errors
                ViewData["ErrorMessage"] = "Lỗi bất ngờ xảy ra, vui lòng thử lại sau";
            }

            // Return view with the matches (which will be an empty list if no data)
            return View(matches);
        }
    }
}
