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
        [HttpPost]
        public async Task<IActionResult> LoginAsync(AdminLoginDto loginDto)
        {
            var token = await _userRepository.AdminLoginAsync(loginDto);
            if (string.IsNullOrEmpty(token)) return BadRequest("Login failed");
            return Ok(token);
        }
        [Authorize(Policy = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok("Authorize");
        }
    }
}
