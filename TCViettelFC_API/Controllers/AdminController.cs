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
        [Authorize(Policy = "admin")]
        [HttpPost("changepass")]
        public async Task<IActionResult> ChangePassword(ChangePassRequest ch)
        {
            var result = await _userRepository.AdminChangePasswordAsync(ch);
            if (result != 1) return BadRequest("Change password failed");
            return Ok("Change password successfully");
        }
    }
}
