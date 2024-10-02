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
        public IActionResult Login()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            var userRole = Request.Cookies["UserRole"];
            if (userRole == "2")
            {
                return RedirectToAction("Home", "Admin");
            }
            else
            {
                return RedirectToAction("Home", "Staff");
            }
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("UserRole");

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
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(1)
                    };

                    Response.Cookies.Append("AuthToken", token.token, cookieOptions);
                    Response.Cookies.Append("UserRole", token.roleId.ToString(), cookieOptions);

                    if (token.roleId == 2)
                    {
                        return RedirectToAction("Home", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Home", "Staff");
                    }

                }
                else
                {
                    ViewBag.Error = "Số điện thoại hoặc mật khẩu sai!";
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
