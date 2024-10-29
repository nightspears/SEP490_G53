using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    }
}
