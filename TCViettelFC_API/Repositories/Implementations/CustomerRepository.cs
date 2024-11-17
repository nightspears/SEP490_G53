using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private static readonly Dictionary<string, (CustomersAccount Customer, string Code, DateTime Expiry)> _pendingRegistrations = new();
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomerRepository(Sep490G53Context context, IConfiguration configuration, IEmailService emailService, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

	
		public async Task<int> RegisterAsync(CustomerRegisterRequest cusRegReq)
        {
            var existedCus = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.Email == cusRegReq.Email);
            if (existedCus != null) return 0;
            try
            {
                var confirmationCode = GenerateConfirmationCode();
                var subject = "Confirmation Code";
                var message = $"Your confirmation code is: {confirmationCode}";
                await _emailService.SendEmailAsync(cusRegReq.Email, subject, message);
                var temporaryCustomer = new CustomersAccount()
                {
                    Email = cusRegReq.Email,
                    Password = cusRegReq.Password,
                    Status = 1
                };
                _pendingRegistrations[cusRegReq.Email] = (temporaryCustomer, confirmationCode, DateTime.UtcNow.AddMinutes(10)); // Set expiry time
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public async Task<int> PostFeedback(FeedbackPostDto feedbackDto)
        {
            var customerId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            Feedback feedback;
            if (customerId == null)
            {
                return 0;
            }

            var customer = new Customer() { AccountId = int.Parse(customerId) };
            await _context.Customers.AddAsync(customer);
            feedback = new Feedback()
            {
                Content = feedbackDto.Content,
                CreatedAt = DateTime.UtcNow,
                Creator = customer,
                Status = 0
            };

            try
            {
                await _context.Feedbacks.AddAsync(feedback);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return 0;
            }


        }
        private string GenerateConfirmationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
		// Method to retrieve a customer by account ID
		public async Task<CustomerAccountDTO?> GetCustomerByAccountIdAsync(int accountId)
		{
			var customerAccount = await _context.CustomersAccounts
				.FirstOrDefaultAsync(ca => ca.CustomerId == accountId);

			if (customerAccount == null) return null;

			return new CustomerAccountDTO
			{
				CustomerId = customerAccount.CustomerId,
				Email = customerAccount.Email,
				Phone = customerAccount.Phone,
				Status = customerAccount.Status
			};
		}

		public class CustomerAccountDTO
		{
			public int CustomerId { get; set; }
			public string? Email { get; set; }
			public string? Phone { get; set; }
			public int? Status { get; set; }
		}
		public async Task<bool> VerifyConfirmationCodeAsync(string email, string code)
        {
            if (_pendingRegistrations.TryGetValue(email, out var storedData))
            {
                var (customer, confirmationCode, expiry) = storedData;
                if (confirmationCode == code && expiry > DateTime.UtcNow)
                {
                    await _context.CustomersAccounts.AddAsync(customer);
                    await _context.SaveChangesAsync();
                    var profile = new Profile()
                    {
                        CustomerId = customer.CustomerId,
                    };
                    await _context.Profiles.AddAsync(profile);
                    await _context.SaveChangesAsync();
                    _pendingRegistrations.Remove(email);
                    return true;
                }
            }

            return false;
        }
        public async Task<CustomerLoginResponse> LoginAsync(CustomerLoginDto dto)
        {
            var customer = await _context.CustomersAccounts
                .FirstOrDefaultAsync(x => x.Email == dto.Email && x.Password == dto.Password);
            if (customer == null) return null;


            string token = string.Empty;
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenDes = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("CustomerId", customer.CustomerId.ToString()),
                new Claim("CustomerEmail", customer.Email.ToString()),
            }),
                Expires = DateTime.UtcNow.AddHours(6),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var jwtToken = jwtHandler.CreateToken(tokenDes);
            token = jwtHandler.WriteToken(jwtToken);

            var response = new CustomerLoginResponse()
            {
                token = token,
                customerId = customer.CustomerId,
                email = customer.Email,
                phone = customer.Phone,
            };
            return response;
        }


        public async Task<ProfileDto?> GetCustomerProfile()
        {

            var customerId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            if (customerId == null) return null;
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.CustomerId == int.Parse(customerId));
            if (profile == null) return null;
            var result = new ProfileDto()
            {
                DateOfBirth = profile.DateOfBirth,
                FullName = profile.FullName,
                Gender = profile.Gender,
            };
            return result;
        }
        public async Task<int> UpdateCustomerProfile(ProfileDto profileDto)
        {
            var customerId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            if (customerId == null) return 0;
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.CustomerId == int.Parse(customerId));
            if (profile == null) return 0;
            profile.FullName = profileDto.FullName;
            profile.Gender = profileDto.Gender;
            profile.DateOfBirth = profileDto.DateOfBirth;
            try
            {
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

		public async Task<List<PersonalAddressDTO>> GetPersonalAddressesByCustomerIdAsync(int customerId)
		{
			// Fetch the list of PersonalAddresses associated with the CustomerId
			var addresses = await _context.PersonalAddresses
				.Where(pa => pa.CustomerId == customerId)
				.ToListAsync();

			// If no addresses are found, return an empty list
			if (addresses == null || !addresses.Any())
			{
				return new List<PersonalAddressDTO>();
			}

			// Map each PersonalAddress to PersonalAddressDTO
			var addressDtos = addresses.Select(address => new PersonalAddressDTO
			{
				AddressId = address.AddressId,
				CityName = address.CityName,
				City = address.City,
				DistrictName = address.DistrictName,
				District = address.District,
				WardName = address.WardName,
				Ward = address.Ward,
				DetailedAddress = address.DetailedAddress,
				Status = address.Status
			}).ToList();

			return addressDtos;
		}
		// Method to insert a PersonalAddress using DTO (manual mapping)
		public async Task<bool> InsertPersonalAddressAsync(PersonalAddressCreateDto personalAddressDto)
		{
			if (personalAddressDto == null)
			{
				return false;
			}

			try
			{
				// Manually map the DTO to the entity
				var personalAddress = new PersonalAddress
				{
					CustomerId = personalAddressDto.CustomerId,
					CityName = personalAddressDto.CityName,
					City = personalAddressDto.City,
					DistrictName = personalAddressDto.DistrictName,
					District = personalAddressDto.District,
					WardName = personalAddressDto.WardName,
					Ward = personalAddressDto.Ward,
					DetailedAddress = personalAddressDto.DetailedAddress,
					Status = personalAddressDto.Status
				};

				// Add the personal address to the database
				await _context.PersonalAddresses.AddAsync(personalAddress);

				// Save changes to the database
				await _context.SaveChangesAsync();

				return true;
			}
			catch (Exception ex)
			{
				// Log the error (you can implement a logging mechanism here)
				Console.WriteLine($"Error inserting personal address: {ex.Message}");
				return false;
			}
		}

	}
}
