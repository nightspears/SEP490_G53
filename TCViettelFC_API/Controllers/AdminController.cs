using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(AdminLoginDto loginDto)
        {
            var response = await _userRepository.AdminLoginAsync(loginDto);
            if (response == null) return BadRequest("Login failed");
            return Ok(response);
        }
        [Authorize(Policy = "admin")]
        [HttpGet("testauthorized")]
        public async Task<IActionResult> GetAsync()
        {
            return Ok("Authorize");
        }
        [HttpPost("changepass")]
        public async Task<IActionResult> ChangePassword(int userId, string oldPass, string newPass)
        {
            var result = await _userRepository.AdminChangePasswordAsync(userId, newPass);
            if (result == 1) return Ok("Change password successfully");
            return BadRequest("Change password failed");
        }
    }
}
