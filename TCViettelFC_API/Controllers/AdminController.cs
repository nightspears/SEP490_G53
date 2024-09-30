using AutoMapper;
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
        private readonly IMapper _mapper;
        public AdminController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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


        // Get a list of users
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var userDTOs = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDTOs);
        }
        [HttpGet("GetRoles")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetRoles()
        {
            var roles = await _userRepository.GetRoleAsync();
            return Ok(roles);
        }

        // Add a new user
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUserAsync(UserCreateDto userCreateDto)
        {
            // Check if the user already exists
            if (_userRepository.ExistedUser(userCreateDto.Email, userCreateDto.Phone))
            {
                return BadRequest("User with the same email or phone already exists.");
            }

            // Check if RoleId is valid
            if (!await _userRepository.IsValidRoleAsync(userCreateDto.RoleId))
            {
                return BadRequest("Invalid RoleId.");
            }

            // Call the repository to add the new user
            await _userRepository.AddUserAsync(userCreateDto);

            // Return success response
            return Ok("User created successfully.");
        }
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            try
            {
               

                // Check if RoleId is valid
                if (!await _userRepository.IsValidRoleAsync(userDto.RoleId))
                {
                    return BadRequest("Invalid RoleId.");
                }
                await _userRepository.UpdateUserAsync(id, userDto);
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error updating user: " + ex.Message);
            }
        }
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userRepository.DeleteUserAsync(id);
                return Ok("User deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Delete failed: " + ex.Message);
            }
        }

    }
}
