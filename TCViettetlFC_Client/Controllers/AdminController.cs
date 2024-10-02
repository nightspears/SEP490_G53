using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
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

        public IActionResult Home()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
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


        public async Task<IActionResult> UserManagement()
        {
            var token = Request.Cookies["AuthToken"];
            var users = await _userService.GetUsersAsync(token);
            var roles = await _userService.GetRolesAsync();

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

            var resultMessage = await _userService.AddUserAsync(userCreateDto, token);

            TempData["Message"] = resultMessage;

            return RedirectToAction("UserManagement");
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

            TempData["Message"] = resultMessage;

            return RedirectToAction("UserManagement");
        }


    }
}
