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
                return RedirectToAction("Home", "Staff");
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
                        Expires = DateTime.UtcNow.AddHours(1)
                    };

                    Response.Cookies.Append("AuthToken", token.token, cookieOptions);
                    Response.Cookies.Append("UserId", token.userId.ToString(), cookieOptions);
                    Response.Cookies.Append("RoleId", token.roleId.ToString(), cookieOptions);

                    if (token.roleId == 2)
                    {
                        return RedirectToAction("Home", "Admin");
                    }
                    else if (token.roleId == 1)
                    {
                        return RedirectToAction("Home", "Staff");
                    }
                    else
                    {
                        return RedirectToAction("Login");
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
