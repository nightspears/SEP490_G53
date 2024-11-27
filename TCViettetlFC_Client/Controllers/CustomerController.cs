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
            SetCustomerInfoInViewData();

            return View();
        }

        public void SetCustomerInfoInViewData()
        {
            string customerId = Request.Cookies["CustomerId"];
            string customerPhone = Request.Cookies["CustomerPhone"];
            string customerEmail = Request.Cookies["CustomerEmail"];

            // Check if the cookies exist and pass them to ViewData
            ViewData["CustomerId"] = customerId ?? string.Empty;
            ViewData["CustomerPhone"] = customerPhone ?? string.Empty;
            ViewData["CustomerEmail"] = customerEmail ?? string.Empty;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(CustomerLoginModel clm)
        {
            if (!ModelState.IsValid) return View();
            var result = await _httpClient.PostAsJsonAsync("customer/login", clm);
            if (result.IsSuccessStatusCode)
            {
                var response = await JsonSerializer.DeserializeAsync<CustomerLoginResponse>(await result.Content.ReadAsStreamAsync());
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("AuthToken", response.token, cookieOptions);
                Response.Cookies.Append("CustomerId", response.customerId.ToString(), cookieOptions);
                Response.Cookies.Append("CustomerEmail", response.email.ToString(), cookieOptions);
                Response.Cookies.Append("CustomerPhone", response.phone?.ToString() ?? string.Empty, cookieOptions);
                return RedirectToAction("Index","Home");
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
            var email = Request.Cookies["EmailForVerification"];
            var result = await _httpClient.PostAsJsonAsync("customer/verify", new { Email = email, evd.Code });
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("Code", "Mã xác nhận không chính xác, vui lòng thử lại");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(CustomerRegisterModel crm)
        {
            var result1 = await _httpClient.GetAsync($"customer/emailexisted?email={crm.Email}");
            if (result1.IsSuccessStatusCode)
            {
                ModelState.AddModelError("Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
                return View();
            }
            if (!ModelState.IsValid) return View();

            var result = await _httpClient.PostAsJsonAsync("customer/register", new { crm.Email, crm.Phone, crm.Password });
            if (result.IsSuccessStatusCode)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(1)
                };
                Response.Cookies.Append("EmailForVerification", crm.Email, cookieOptions);
                return RedirectToAction("Verify");
            }
            return View();

        }

    }
}
