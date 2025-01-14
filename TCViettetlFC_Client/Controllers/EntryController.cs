﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TCViettelFC_Client.ApiServices;

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
            return View();
        }
        [HttpGet("verifyticket/{ticketId}")]
        public async Task<IActionResult> VerifyTicket(int ticketId)
        {
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
            var token = Request.Cookies["AuthToken"];

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"https://localhost:5000/api/entry/verifysupitem/{orderId}");
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    ViewBag.Result = await response.Content.ReadAsStringAsync();
                }

            }
            return View();
        }
    }
}
