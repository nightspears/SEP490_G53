using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using TCViettetlFC_Client.Models;
using TCViettelFC_Client.ApiServices;
using Newtonsoft.Json;
namespace TCViettetlFC_Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IApiHelper _apiHelper;

        public HomeController(ILogger<HomeController> logger , IHttpClientFactory httpClientFactory, IApiHelper apiHelper)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiHelper = apiHelper;
        }

        public async Task<IActionResult> Index()
        {

            string NewsUri = "https://localhost:5000/api/New/GetAllNews?top=8&$orderby= createdAt desc";
            var responseNews = await _httpClient.GetAsync(NewsUri);
            List<GetAllNewViewModel> lstNews = new List<GetAllNewViewModel>();
            if (responseNews.IsSuccessStatusCode)
            {
                var jsonData = await responseNews.Content.ReadAsStringAsync();
                lstNews = JsonConvert.DeserializeObject<List<GetAllNewViewModel>>(jsonData);
            }


            string PlayerUri = "https://localhost:5000/api/Players";
            var responsePlayer = await _httpClient.GetAsync(PlayerUri);
            List<PlayerViewModel> lstPlayer = new List<PlayerViewModel>();
            if (responsePlayer.IsSuccessStatusCode)
            {
                var jsonData = await responsePlayer.Content.ReadAsStringAsync();
                lstPlayer = JsonConvert.DeserializeObject<List<PlayerViewModel>>(jsonData);
            }

            ViewBag.News = lstNews;
            ViewBag.Player = lstPlayer;
            return View();


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
