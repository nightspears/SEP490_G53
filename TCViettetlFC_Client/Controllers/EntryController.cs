using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    [Route("Entry")]
    public class EntryController : Controller
    {
        private readonly IApiHelper _apiHelper;
        public EntryController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public IActionResult Home()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "3")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        [HttpGet("verifyticket/{ticketId}")]
        public async Task<IActionResult> VerifyTicket(int ticketId)
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "3")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            var token = Request.Cookies["AuthToken"];

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"https://localhost:5000/api/entry/verifyticket/{ticketId}");
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Valid";
                }
                else
                {
                    ViewBag.Result = "Invalid";
                }

            }
            return View();
        }
        [HttpGet("verifyitem/{orderId}")]
        public async Task<IActionResult> VerifyItem(int orderId)
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "3")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            var token = Request.Cookies["AuthToken"];

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"https://localhost:5000/api/entry/verifysupitem/{orderId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await JsonSerializer.DeserializeAsync<List<VerifySupDto>>(await response.Content.ReadAsStreamAsync());
                    return View(result);
                }
                else
                {
                    ViewBag.Result = await response.Content.ReadAsStringAsync();
                    return View();
                }

            }

        }
    }
}
