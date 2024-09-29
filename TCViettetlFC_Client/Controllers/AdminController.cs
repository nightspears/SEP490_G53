using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
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
        public IActionResult ChangePassword()
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
            var response = await _httpClient.GetAsync("admin/testauthorized");
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Authorize = "Authorized";
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

            var response = await _httpClient.PostAsJsonAsync("admin/login", adminLoginViewModel);
            if (response.IsSuccessStatusCode)
            {
                var token = await JsonSerializer.DeserializeAsync<TokenModel>(await response.Content.ReadAsStreamAsync());
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Prevent access via JavaScript
                    Secure = true, // Only send over HTTPS
                    SameSite = SameSiteMode.Strict, // Strict cookie policy to prevent CSRF
                    Expires = DateTime.UtcNow.AddHours(1) // Adjust the expiration as necessary
                };

                Response.Cookies.Append("AuthToken", token.token, cookieOptions);

                return RedirectToAction("Index", "Home");
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ChangePass(ChangePasswordModel adminLoginViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("admin/login", adminLoginViewModel);
            if (response.IsSuccessStatusCode)
            {
                var token = await JsonSerializer.DeserializeAsync<TokenModel>(await response.Content.ReadAsStreamAsync());
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Prevent access via JavaScript
                    Secure = true, // Only send over HTTPS
                    SameSite = SameSiteMode.Strict, // Strict cookie policy to prevent CSRF
                    Expires = DateTime.UtcNow.AddHours(1) // Adjust the expiration as necessary
                };

                Response.Cookies.Append("AuthToken", token.token, cookieOptions);

                return RedirectToAction("Index", "Home");
            }
            return View();

        }

    }
}
