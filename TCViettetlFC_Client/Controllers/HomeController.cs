using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;

namespace TCViettetlFC_Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;
        private readonly FeedbackService _feedbackService;
        private readonly IApiHelper _apiHelper;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, FeedbackService feedbackService, IApiHelper apiHelper)
        {
            _logger = logger;
            _httpClient = httpClient;
            _feedbackService = feedbackService;
            _apiHelper = apiHelper;
        }

        public async Task<IActionResult> Index()
        {
            string requestUri = "https://localhost:5000/api/New/GetAllNews";
            List<GetAllNewViewModel> newsList = new();

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    newsList = JsonConvert.DeserializeObject<List<GetAllNewViewModel>>(jsonData);

                    // Sort news by CreatedAt in descending order and take the latest 5
                    newsList = newsList
                        .OrderByDescending(news => news.CreatedAt)
                        .Take(5)
                        .ToList();
                }
                else
                {
                    _logger.LogWarning($"Failed to retrieve news. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching news.");
            }

            return View(newsList); // Pass latest news to the view
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
