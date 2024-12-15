using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public async Task<IActionResult> ApproveNew(string searchTerm, int? sortOption)
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "2")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            string requestUri = "https://tcvtfcapi.azurewebsites.net/api/New/GetAllNews";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                requestUri += $"?$filter=contains(Title,'{searchTerm}')";
            }
            else
            {
                requestUri += "?";
            }
            switch (sortOption)
            {
                case 0:
                    requestUri += "?";
                    break;
                case 1:
                    requestUri += "&$orderby=Title asc";
                    break;
                case 2:
                    requestUri += "&$orderby=CreatedAt asc";
                    break;
                case 3:
                    requestUri += "&$orderby=CreatedAt desc";
                    break;
                case 4:
                    requestUri += "&$filter=Status eq 0";
                    break;
                case 5:
                    requestUri += "&$filter=Status eq 1";
                    break;

            }
            var response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var newList = JsonConvert.DeserializeObject<List<GetAllNewViewModel>>(jsonData);
                return View(newList);
            }


            return View("Error");


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


            var response = await _httpClient.GetAsync($"https://tcvtfcapi.azurewebsites.net/api/New/updatestatus/{newsId}?newStatus={newStatus}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ApproveNew");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return View("Error", errorMessage);
        }


    }
}
