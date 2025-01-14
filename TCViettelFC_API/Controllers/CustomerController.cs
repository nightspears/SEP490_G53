﻿using Microsoft.AspNetCore.Mvc;
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
		[HttpGet("getCustomerAccountById/{accountId}")]
		public async Task<IActionResult> GetCustomerAccountById(int accountId)
		{
			var customerAccount = await _customerRepository.GetCustomerByAccountIdAsync(accountId);
			if (customerAccount == null)
			{
				return NotFound("Customer account not found");
			}
			return Ok(customerAccount);
		}

		// API to get a list of PersonalAddressDTOs by CustomerId
		[HttpGet("address/{customerId}")]
		public async Task<ActionResult<List<PersonalAddressDTO>>> GetPersonalAddresses(int customerId)
		{
			var addressDtos = await _customerRepository.GetPersonalAddressesByCustomerIdAsync(customerId);

			if (addressDtos == null || !addressDtos.Any())
			{
				return NotFound(new { message = "No addresses found for the given customer." });
			}

			return Ok(addressDtos);
		}

		// API Endpoint to add a new personal address
		[HttpPost("add-address")]
		public async Task<IActionResult> AddPersonalAddress([FromBody] PersonalAddressCreateDto personalAddressDto)
		{
			// Check if the DTO is valid
			if (personalAddressDto == null)
			{
				return BadRequest("Invalid address data.");
			}

			// Call the repository to insert the personal address
			var result = await _customerRepository.InsertPersonalAddressAsync(personalAddressDto);

			if (result)
			{
				return Ok("Address added successfully.");
			}

			return BadRequest("Failed to add address.");
		}

	}
}
