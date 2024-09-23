using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null) return BadRequest("Data cannot be null");
            if (_userRepository.ExistedUser(registerDto.UserName, registerDto.PhoneNumber))
                return BadRequest("Username already existed!");
            var result = await _userRepository.RegisterAsync(registerDto);
            if (result == 1) return Ok("Register successfully");
            else return BadRequest("Register failed");
        }
        [HttpPost("/login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var token = await _userRepository.LoginAsync(loginDto);
            if (string.IsNullOrEmpty(token)) return BadRequest("Login failed");
            return Ok(token);
        }
        [Authorize]
        [HttpGet("getbyuser")]
        public async Task<IActionResult> GetByUserAsync()
        {
            return Ok("Hello user");
        }
    }
}
