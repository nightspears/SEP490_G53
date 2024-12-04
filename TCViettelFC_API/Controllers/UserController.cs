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
        [HttpGet("userprofile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var result = await _userRepository.GetUserProfile();
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateUserProfile(UserProfileDto dto)
        {
            var result = await _userRepository.UpdateUserProfile(dto);
            if (result) return Ok(result);
            return Conflict("Cập nhật thất bại");
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var response = await _userRepository.LoginAsync(loginDto);
            if (response == null) return BadRequest("Login failed");
            return Ok(response);
        }
        [HttpGet("checkmail/{email}")]
        public IActionResult CheckExistedMailAsync(string email)
        {
            var result = _userRepository.CheckExistedEmailAsync(email);
            if (result == true) return Ok();
            return Conflict("Email không tồn tại");
        }
        [HttpGet("sendcode/{email}")]
        public async Task<IActionResult> SendConfimationEmailAsync(string email)
        {
            var result = await _userRepository.SendConfirmationCodeAsync(email);
            if (result == true) return Ok("Gửi mã thành công");
            else return BadRequest("Gửi mã thất bại");
        }
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyConfirmationCodeAsync([FromBody] VerifyConfirmationCodeRequest request)
        {
            var isValid = await _userRepository.VerifyConfirmationCodeAsync(request.Email, request.Code);
            if (isValid)
            {
                return Ok("Email confirmed successfully");
            }
            return BadRequest("Invalid or expired confirmation code");
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPassRequest request)
        {
            var res = _userRepository.CheckExistedEmailAsync(request.Email);
            if (!res) return Conflict("Email không tồn tại");
            var resu = await _userRepository.ResetPasswordAsync(request.Email, request.NewPass);
            if (resu == -1) return Conflict("Mật khẩu mới không được trùng với mật khẩu cũ");
            if (resu == 1) return Ok("Đặt lại mật khẩu thành công");
            return BadRequest("Đặt lại mật khẩu thất bại");
        }
    }
}
