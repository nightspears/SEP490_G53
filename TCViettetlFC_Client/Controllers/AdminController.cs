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
        private async Task<string> GetTotalCustomerAsync()
        {
            var response = await _httpClient.GetAsync("admin/totalcustomer");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        private async Task<string> GetTotalTicketRevenueAsync()
        {
            var response = await _httpClient.GetAsync("admin/totalticketrevenue");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        private async Task<string> GetTotalProductRevenueAsync()
        {
            var response = await _httpClient.GetAsync("admin/totalproductrevenue");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        private async Task<string> GetTotalSoldTicketsAsync()
        {
            var response = await _httpClient.GetAsync("admin/totalsoldtickets");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        public async Task<IActionResult> Home()
        {

            var cookies = Request.Cookies["RoleId"];
            if (cookies != "2")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            ViewBag.TotalCustomer = await GetTotalCustomerAsync();
            ViewBag.TotalTicketRevenue = await GetTotalTicketRevenueAsync();
            ViewBag.TotalProductRevenue = await GetTotalProductRevenueAsync();
            ViewBag.TotalSoldTickets = await GetTotalSoldTicketsAsync();

            return View();


        }

        public IActionResult ChangePassword()
        {
            var cookies = Request.Cookies["RoleId"];
            if (string.IsNullOrEmpty(cookies))
            {
                return RedirectToAction("Index", "Forbidden");
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
                TempData["Notification"] = "Đổi mật khẩu thành công";
                TempData["NotificationType"] = "success";
            }
            else
            {
                TempData["Notification"] = "Đổi mật khẩu thất bại";
                TempData["NotificationType"] = "error";
            }
            return RedirectToAction("ChangePassword");

        }


        public async Task<IActionResult> UserManagement()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "2")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            var token = Request.Cookies["AuthToken"];
            var users = await _userService.GetUsersAsync(token);
            var roles = await _userService.GetRolesAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var model = new UserManagementViewModel
            {
                Users = users.ToList(),
                Roles = roles.ToList()
            }; ;

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
