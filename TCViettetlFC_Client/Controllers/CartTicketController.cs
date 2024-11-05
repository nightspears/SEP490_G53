using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class CartTicketController : Controller
    {
       
        private readonly HttpClient _httpClient;

        public CartTicketController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");

        }
        public async Task<IActionResult> CartTicket()
        {
            string customerId = Request.Cookies["CustomerId"];
            
            ViewData["CustomerId"] = customerId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] TicketOrderRequest payload)
        {
            // Retrieve customer ID from payload or ViewData if needed
            var customerId = payload.customerId;
            payload.orderDate = DateTime.Now; // ISO 8601 format

            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"https://localhost:5000/api/TicketOrder?customerId={customerId}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thanh toán thành công! Đơn hàng của bạn đã được xử lý." });
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi thanh toán: {ex.Message}" });
            }
        }
    }
}
