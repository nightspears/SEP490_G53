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
            var checkEmail = await _customerRepository.CheckExistedCustomerEmail(cusRegReq.Email);
            if (checkEmail == 0) return BadRequest("Email đã tồn tại");
            var result = await _customerRepository.RegisterAsync(cusRegReq);
            if (result == 1) return Ok("Đăng ký thành công");
            return BadRequest("Đăng ký thất bại");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] CustomerLoginDto customerLoginDto)
        {
            var result = await _customerRepository.LoginAsync(customerLoginDto);
            if (result == null) return BadRequest("Đăng nhập thất bại");
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
        [HttpGet("emailexisted")]
        public async Task<IActionResult> GetExistedCustomer(string email)
        {
            var cus = await _customerRepository.CheckExistedCustomerEmail(email);
            if (cus == 0) return Ok("Email existed");
            return BadRequest("Email not existed");
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

        [HttpDelete("DeletePersonalAddress/{id}")]
        public async Task<IActionResult> DeletePersonalAddress(int id)
        {
            try
            {
                // Call the repository method to delete the personal address
                var result = await _customerRepository.DeletePersonalAddressAsync(id);

                if (result)
                {
                    // Return 200 OK response if deletion is successful
                    return Ok(new { success = true, message = "Personal address deleted successfully." });
                }
                else
                {
                    // Return 404 Not Found if the personal address does not exist
                    return NotFound(new { success = false, message = "Personal address not found." });
                }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error for unexpected errors
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the personal address.", details = ex.Message });
            }
        }


    }
}
