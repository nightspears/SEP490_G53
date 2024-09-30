using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TCViettelFC_Client.Services;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        public AdminController(IHttpClientFactory httpClientFactory, UserService userService)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _userService = userService;
        }
        public IActionResult Login()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            return RedirectToAction("Home", "Admin");
        }
        public IActionResult Home()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }

        }
        public IActionResult Profile()
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

                return RedirectToAction("Home", "Admin");
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("admin/changepass", new { OldPass = changePasswordModel.OldPassword, NewPass = changePasswordModel.NewPassword });
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Password changed successfully";
            }
            else
            {
                ViewBag.Message = "Failed to change password";
            }
            return RedirectToAction("Profile");

        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("AuthToken");

            return RedirectToAction("Login", "Admin");
        }

        public async Task<IActionResult> UserManagement()
        {
            var token = Request.Cookies["AuthToken"];
            var users = await _userService.GetUsersAsync(token);
            var roles = await _userService.GetRolesAsync();

            // Assuming you have a ViewModel to hold both users and roles
            var model = new UserManagementViewModel
            {
                Users = (List<UserViewModel>)users,
                Roles = (List<RoleViewModel>)roles
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateDto userCreateDto)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            // Call the service to add the user
            var resultMessage = await _userService.AddUserAsync(userCreateDto, token);

            // Add a success or error message to TempData
            TempData["Message"] = resultMessage;

            return RedirectToAction("UserManagement"); // Redirect to the user management page
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdateDto userUpdateDto)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            var result = await _userService.UpdateUserAsync(userUpdateDto.UserId, userUpdateDto, token);

          
                TempData["Message"] = result;
                return RedirectToAction("UserManagement");
           
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            var resultMessage = await _userService.DeleteUserAsync(id, token);

            // Add a success or error message to TempData
            TempData["Message"] = resultMessage;

            return RedirectToAction("UserManagement");
        }


    }
}
