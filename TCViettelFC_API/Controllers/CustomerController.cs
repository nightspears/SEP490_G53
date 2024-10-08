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

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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
    }
}
