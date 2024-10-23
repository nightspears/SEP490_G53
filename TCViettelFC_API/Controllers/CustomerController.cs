using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITicketUtilRepository _ticketUtilRepository;

        public CustomerController(ICustomerRepository customerRepository, ITicketUtilRepository ticketUtilRepository)
        {
            _customerRepository = customerRepository;
            _ticketUtilRepository = ticketUtilRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CustomerRegisterRequest cusRegReq)
        {
            var result = await _customerRepository.RegisterAsync(cusRegReq);
            if (result == 1) return Ok("Register successfully");
            return BadRequest("Register failed");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] CustomerLoginDto customerLoginDto)
        {
            var result = await _customerRepository.LoginAsync(customerLoginDto);
            if (result == null) return BadRequest("Login failed");
            return Ok(result);
        }
        [HttpGet("profile")]
        public async Task<IActionResult> GetCustomerProfile()
        {
            var result = await _customerRepository.GetCustomerProfile();
            return Ok(result);
        }


        [HttpPost("verify")]
        public async Task<IActionResult> VerifyConfirmationCodeAsync([FromBody] VerifyConfirmationCodeRequest request)
        {
            var isValid = await _customerRepository.VerifyConfirmationCodeAsync(request.Email, request.Code);
            if (isValid)
            {
                return Ok("Email confirmed successfully");
            }
            return BadRequest("Invalid or expired confirmation code");
        }
        [HttpPut("updateprofile")]
        public async Task<IActionResult> UpdateProfile(ProfileDto profile)
        {
            var result = await _customerRepository.UpdateCustomerProfile(profile);
            if (result == 1)
            {
                return Ok("Profile updated successfully");
            }
            return BadRequest("Failed to update profile");
        }
        [HttpPost("sendmailticket")]
        public async Task<IActionResult> SendTicketViaMailAsync()
        {
            var result = await _ticketUtilRepository.SendTicketViaEmailAsync([1, 2]);
            if (result == 1) return Ok();
            return BadRequest();
        }
        [Authorize(Policy = "entry")]
        [HttpGet("verifyticket/{ticketId}")]
        public async Task<IActionResult> VerifyTicketAsync(int ticketId)
        {
            var ticket = await _ticketUtilRepository.GetOrderedTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound("Ticket not found");
            var result = await _ticketUtilRepository.VerifyTicketAsync(ticket);
            if (result == 1) return Ok("Ticket verified successfully");
            return BadRequest("Ticket not valid");
        }
    }
}
