using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class AreaController : Controller
    {
        private readonly HttpClient _httpClient;

        public AreaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");

        }
        public async Task<IActionResult> Index()
        {
            string customerId = Request.Cookies["CustomerId"];
            string requestUri = "https://localhost:5000/api/SupplementaryItem";
            var response = await _httpClient.GetAsync(requestUri);
            List<SupplementaryIteamViewModel> newList = new List<SupplementaryIteamViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                newList = JsonConvert.DeserializeObject<List<SupplementaryIteamViewModel>>(jsonData);
            }
            
            ViewData["NewList"] = newList;
            ViewData["CustomerId"] = customerId;
            return View();
        }

        



    }
}
