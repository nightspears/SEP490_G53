using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using TCViettetlFC_Client.Models;
namespace TCViettetlFC_Client.Controllers
{
    public class AdminNewsController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminNewsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> ApproveNew()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                var response = await _httpClient.GetAsync("https://localhost:5000/api/New/GetAllNews");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();

                    var newsList = JsonConvert.DeserializeObject<List<GetAllNewViewModel>>(jsonData);

                    return View(newsList);
                }
                return View("Error");
            }
           
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int newsId, int newStatus)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            
            var response = await _httpClient.GetAsync($"https://localhost:5000/api/New/updatestatus/{newsId}?newStatus={newStatus}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ApproveNew");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return View("Error", errorMessage);
        }
        

    }
}
