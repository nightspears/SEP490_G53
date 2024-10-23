using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IApiHelper _apiHelper;

        public CustomerController(IHttpClientFactory httpClientFactory, IApiHelper apiHelper)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiHelper = apiHelper;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            ViewBag.Profile = await _apiHelper.GetApiResponseAsync<CustomerProfileModel>("customer/profile");
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(CustomerLoginModel clm)
        {
            var result = await _httpClient.PostAsJsonAsync("customer/login", clm);
            if (result.IsSuccessStatusCode)
            {
                var response = await JsonSerializer.DeserializeAsync<CustomerLoginResponse>(await result.Content.ReadAsStreamAsync());
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("AuthToken", response.token, cookieOptions);
                Response.Cookies.Append("CustomerId", response.customerId.ToString(), cookieOptions);
                return RedirectToAction("Home");
            }
            else
            {
                ViewBag.Error = "Email hoặc mật khẩu sai!";
                return View(clm);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(CustomerProfileModel model)
        {

            var result = await _apiHelper.UpdateApiResponseAsync("customer/updateprofile", model);
            ViewBag.Result = result;
            return RedirectToAction("Profile");
        }
        public IActionResult Verify()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Verify(EmailVerificationModel evd)
        {
            var result = await _httpClient.PostAsJsonAsync("customer/verify", evd);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(CustomerRegisterModel crm)
        {
            if (!ModelState.IsValid) return View();
            var result = await _httpClient.PostAsJsonAsync("customer/register", new { crm.Email, crm.Password });
            if (result.IsSuccessStatusCode)
            {
                TempData["Email"] = crm.Email;
                return RedirectToAction("Verify");
            }
            return View();

        }

    }
}
