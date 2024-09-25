using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Login1()
        {
            return View();
        }
        public async Task<IActionResult> GetAuthorize()
        {

            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("Admin");
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Authorize = await response.Content.ReadAsStringAsync();
            }
            else
            {
                ViewBag.Authorize = "Unauthorized";
            }



            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel adminLoginViewModel)
        {

            var response = await _httpClient.PostAsJsonAsync("https://localhost:5000/api/Admin", adminLoginViewModel);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Prevent access via JavaScript
                    Secure = true, // Only send over HTTPS
                    SameSite = SameSiteMode.Strict, // Strict cookie policy to prevent CSRF
                    Expires = DateTime.UtcNow.AddHours(1) // Adjust the expiration as necessary
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return RedirectToAction("Index", "Home");
            }
            return View();

        }
    }
}
