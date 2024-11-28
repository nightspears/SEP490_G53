using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
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
            var cookie = Request.Cookies["CustomerId"];
            if (cookie != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Forgot()
        {
            var cookie = Request.Cookies["CustomerId"];
            if (cookie != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public async Task<IActionResult> SendForgotMail(CustomerSendMailModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _httpClient.GetAsync($"customer/sendcode/{model.Email}");
            if (result.IsSuccessStatusCode)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(1)
                };
                Response.Cookies.Append("EmailForgot", model.Email, cookieOptions);
                return RedirectToAction("Verify");
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Profile()
        {
            var cookie = Request.Cookies["CustomerId"];
            if (cookie == null)
            {
                return RedirectToAction("Login");
            }
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
            var cookie = Request.Cookies["CustomerId"];
            if (cookie != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpGet]
        public IActionResult ChangePass()
        {
            var cookie = Request.Cookies["CustomerId"];
            if (cookie == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePass(CustomerChangePassRequest ch)
        {
            if (!ModelState.IsValid)
            {
                return View(ch);
            }
            var cookie = Request.Cookies["AuthToken"];
            if (cookie == null)
            {
                return RedirectToAction("Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookie);
            var result = await _httpClient.PostAsJsonAsync("customer/changepass", new { OldPass = ch.OldPass, NewPass = ch.NewPassword });

            var msg = await JsonSerializer.DeserializeAsync<CustomerChangePassResponse>(await result.Content.ReadAsStreamAsync());
            if (result.IsSuccessStatusCode)
            {
                ViewBag.Notify = msg.message;
                return View(ch);
            }
            else
            {
                if (msg.message.Contains("cũ"))
                {
                    ModelState.AddModelError("OldPass", msg.message);
                    return View(ch);
                }
                else if (msg.message.Contains("mới"))
                {
                    ModelState.AddModelError("NewPassword", msg.message);
                    return View(ch);
                }
                else
                {
                    TempData["ChNotify"] = msg.message;
                    return View(ch);
                }

            }


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
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("AuthToken", response.token, cookieOptions);
                Response.Cookies.Append("CustomerId", response.customerId.ToString(), cookieOptions);
                Response.Cookies.Append("CustomerEmail", response.email.ToString(), cookieOptions);
                Response.Cookies.Append("CustomerPhone", response.phone?.ToString() ?? string.Empty, cookieOptions);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Email hoặc mật khẩu sai!";
                return View(clm);
            }
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(CustomerForgotPassModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var email1 = Request.Cookies["EmailForgot"];
            if (email1 != null)
            {
                var result = await _httpClient.PostAsJsonAsync("customer/resetpassword", new { Email = email1, NewPass = model.Password });
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Code", "Mã xác nhận không chính xác, vui lòng thử lại");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(CustomerProfileModel model)
        {

            var result = await _apiHelper.UpdateApiResponseAsync("customer/updateprofile", model);
            ViewBag.Result = result;
            return RedirectToAction("Profile");
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("CustomerId");
            Response.Cookies.Delete("CustomerEmail");
            Response.Cookies.Delete("CustomerPhone");
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("EmailForVerification");
            return RedirectToAction("Login");
        }

        public IActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(EmailVerificationModel evd)
        {
            if (!ModelState.IsValid) return View();
            var email = Request.Cookies["EmailForVerification"];
            if (email != null)
            {
                var result = await _httpClient.PostAsJsonAsync("customer/verify", new { Email = email, evd.Code });
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Code", "Mã xác nhận không chính xác, vui lòng thử lại");
                    return View();
                }
            }
            else
            {
                var email1 = Request.Cookies["EmailForgot"];
                if (email1 != null)
                {
                    var result = await _httpClient.PostAsJsonAsync("customer/verify", new { Email = email1, evd.Code });
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ResetPassword");
                    }
                    else
                    {
                        ModelState.AddModelError("Code", "Mã xác nhận không chính xác, vui lòng thử lại");
                        return View();
                    }
                }
                else
                {
                    return View();
                }

            }


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

            var result = await _httpClient.PostAsJsonAsync("customer/register", new { crm.Email, crm.FullName, crm.Phone, crm.Password });
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
