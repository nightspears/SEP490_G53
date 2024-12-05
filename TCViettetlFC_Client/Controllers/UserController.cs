using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public IActionResult ForgotPassword()
        {
            var token = Request.Cookies["RoleId"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            if (token.Equals("2"))
            {
                return RedirectToAction("Home", "Admin");
            }
            if (token.Equals("1"))
            {
                return RedirectToAction("ProductManagement", "Staff");
            }
            if (token.Equals("3"))
            {
                return RedirectToAction("Home", "Entry");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassUserModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var check = await _httpClient.GetAsync($"user/checkmail/{model.Email}");
            if (!check.IsSuccessStatusCode)
            {
                ModelState.AddModelError("Email", "Không tồn tại tài khoản với email này");
                return View(model);
            }
            var result = await _httpClient.GetAsync($"user/sendcode/{model.Email}");
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
                ModelState.AddModelError("Email", "Lỗi bất ngờ xảy ra.");
                return View(model);
            }
        }
        public IActionResult Verify()
        {
            var token = Request.Cookies["RoleId"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            if (token.Equals("2"))
            {
                return RedirectToAction("Home", "Admin");
            }
            if (token.Equals("1"))
            {
                return RedirectToAction("ProductManagement", "Staff");
            }
            if (token.Equals("3"))
            {
                return RedirectToAction("Home", "Entry");
            }
            return View();
        }
        public async Task<IActionResult> UpdateProfile()
        {
            var token = Request.Cookies["RoleId"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Forbidden");
            }
            var result = await _httpClient.GetAsync("user/userprofile");
            if (result.IsSuccessStatusCode)
            {
                var model = await JsonSerializer.DeserializeAsync<UserProfileModel>(await result.Content.ReadAsStreamAsync());
                return View(model);
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _httpClient.PostAsJsonAsync("user/updateprofile", model);
            if (result.IsSuccessStatusCode)
            {
                ViewBag.Notify = "Cập nhật thông tin thành công";
                return View(model);
            }
            else
            {
                ViewBag.Notify = "Cập nhật thông tin thất bại";
                return View();
            }


        }
        [HttpPost]
        public async Task<IActionResult> Verify(EmailVerificationModel evd)
        {

            var email1 = Request.Cookies["EmailForgot"];
            if (email1 != null)
            {
                var result = await _httpClient.PostAsJsonAsync("user/verify", new { Email = email1, evd.Code });
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
        public IActionResult ResetPassword()
        {
            var token = Request.Cookies["RoleId"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            if (token.Equals("2"))
            {
                return RedirectToAction("Home", "Admin");
            }
            if (token.Equals("1"))
            {
                return RedirectToAction("ProductManagement", "Staff");
            }
            if (token.Equals("3"))
            {
                return RedirectToAction("Home", "Entry");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserResetPassModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var email1 = Request.Cookies["EmailForgot"];
            if (email1 != null)
            {
                var result = await _httpClient.PostAsJsonAsync("user/resetpassword", new { Email = email1, NewPass = model.Password });
                if (result.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("EmailForgot");
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
        public IActionResult Login()
        {
            var token = Request.Cookies["RoleId"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            if (token.Equals("2"))
            {
                return RedirectToAction("Home", "Admin");
            }
            else if (token.Equals("1"))
            {
                return RedirectToAction("ProductManagement", "Staff");
            }
            else if (token.Equals("3"))
            {
                return RedirectToAction("Home", "Entry");
            }
            else
            {
                return View();
            }

        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RoleId");
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("SessionTimer");

            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel userLoginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginViewModel);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("user/login", userLoginViewModel);

                if (response.IsSuccessStatusCode)
                {
                    var token = await JsonSerializer.DeserializeAsync<TokenModel>(await response.Content.ReadAsStreamAsync());

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,

                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddHours(4)
                    };

                    Response.Cookies.Append("AuthToken", token.token, cookieOptions);
                    Response.Cookies.Append("UserId", token.userId.ToString(), cookieOptions);
                    Response.Cookies.Append("RoleId", token.roleId.ToString(), cookieOptions);
                    Response.Cookies.Append("SessionTimer", DateTime.UtcNow.AddHours(4).ToString(), cookieOptions);

                    if (token.roleId == 2)
                    {
                        return RedirectToAction("Home", "Admin");
                    }
                    else if (token.roleId == 1)
                    {
                        if (token.status == 0)
                        {
                            ViewBag.Error = "Tài khoản này không còn hoạt động";
                            return View(userLoginViewModel);
                        }
                        return RedirectToAction("ProductManagement", "Staff");
                    }
                    else if (token.roleId == 3)
                    {
                        return RedirectToAction("Home", "Entry");
                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }

                }
                else
                {
                    ViewBag.Error = "Email hoặc mật khẩu sai!";
                    return View(userLoginViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi bất ngờ xảy ra. Vui lòng thử lại";
                return View(userLoginViewModel);
            }
        }

    }
}
