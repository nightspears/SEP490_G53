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
            if (registerDto == null) return BadRequest(new { Message = "Data cannot be null" });
            if (_userRepository.ExistedUser(registerDto.UserName)) return BadRequest(new { Message = "Username already existed!" });
            var result = await _userRepository.RegisterAsync(registerDto);
            if (result == 1) return Ok(new { Message = "Register successfully" });
            else return BadRequest(new { Message = "Register failed" });
        }
    }
}
