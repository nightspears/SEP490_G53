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

        [HttpPost]
        public async Task<IActionResult> SubmitTicketOrder([FromBody] TicketOrderRequest request)
        {
            try
            {
                string url = request.customerId.HasValue
                    ? $"https://localhost:5000/api/TicketOrder?customerId={request.customerId}"
                    : "https://localhost:5000/api/TicketOrder";

                var payload = JsonConvert.SerializeObject(request);
                Console.WriteLine("Payload JSON gửi đến API: " + payload);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorData = await response.Content.ReadAsStringAsync();
                    Console.Error.WriteLine("Chi tiết lỗi từ API:", errorData);
                    return BadRequest("Đặt vé không thành công.");
                }

                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Đặt vé thành công:", result);

                return Json(new { success = true, message = "Đặt vé thành công!" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Lỗi:", ex);
                return BadRequest("Đã xảy ra lỗi trong quá trình đặt vé.");
            }
        }



    }
}
