using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        private readonly IHttpContextAccessor _contextAccessor;

        public CustomerRepository(Sep490G53Context context, IConfiguration configuration, IEmailService emailService, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public async Task<List<TicketOrderHistoryDto>> GetTicketOrderHistory()
        {
            var cusId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            if (cusId == null) return null;
            var cus = await _context.Customers.Where(x => x.AccountId == int.Parse(cusId)).ToListAsync();
            if (cus == null) return null;
            List<TicketOrder> ticketOrders = new();
            foreach (var c in cus)
            {
                var order = await _context.TicketOrders.FirstOrDefaultAsync(x => x.CustomerId == c.CustomerId);
                if (order != null)
                {
                    ticketOrders.Add(order);
                }

            }

            List<TicketOrderHistoryDto> results = new List<TicketOrderHistoryDto>();
            foreach (var order in ticketOrders)
            {

                results.Add(new TicketOrderHistoryDto()
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                });
            }
            return results;
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            return Convert.ToBase64String(hashBytes);
        }
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            using var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i]) return false;
            }
            return true;
        }




        public async Task<int> CheckExistedCustomerEmail(string email)
        {
            var existedCus = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.Email == email);
            if (existedCus != null) return 0;
            return 1;
        }
        public async Task<int> ResetPasswordAsync(string email, string newPass)
        {
            var cus = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.Email == email);
            if (cus == null) return 0;
            if (VerifyPassword(newPass, cus.Password)) return -1;

            try
            {
                cus.Password = HashPassword(newPass);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public async Task<bool> SendConfirmationCodeAsync(string email)
        {
            var customer = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.Email == email);
            if (customer != null)
            {
                var confirmationCode = GenerateConfirmationCode();
                var subject = "Mã xác nhận tài khoản";
                var message = $"Mã xác nhận của bạn là: {confirmationCode}";
                await _emailService.SendEmailAsync(email, subject, message);
                customer.ConfirmationCode = confirmationCode;
                customer.CodeExpiry = DateTime.UtcNow.AddMinutes(15);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<string> RegisterAsync(CustomerRegisterRequest cusRegReq)
        {
            try
            {


                var customer = new CustomersAccount()
                {
                    Email = cusRegReq.Email,
                    Password = HashPassword(cusRegReq.Password),
                    FullName = cusRegReq.Fullname,
                    Phone = cusRegReq.Phone,
                    Status = 0
                };

                await _context.CustomersAccounts.AddAsync(customer);
                await _context.SaveChangesAsync();



                return cusRegReq.Email;
            }
            catch
            {
                return "";
            }
        }

        public async Task<int> ChangePasswordAsync(ChangePassRequest changePassRequest)
        {
            if (changePassRequest == null) return 0;
            var cusId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            if (cusId == null) return 0;
            var cus = await _context.CustomersAccounts.FindAsync(int.Parse(cusId));
            if (cus == null) return 0;
            if (!VerifyPassword(changePassRequest.OldPass, cus.Password)) return -1;
            if (VerifyPassword(changePassRequest.NewPass, cus.Password)) return -2;

            try
            {
                cus.Password = HashPassword(changePassRequest.NewPass);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public async Task<int> PostFeedback(FeedbackPostDto feedbackDto)
        {
            if (feedbackDto == null) return 0;
            var customerId = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return 0;
            }

            var customer = new Customer { AccountId = int.Parse(customerId) };
            await _context.Customers.AddAsync(customer);

            var feedback = new Feedback
            {
                Content = feedbackDto.Content,
                CreatedAt = DateTime.UtcNow,
                Creator = customer,
                Status = 0
            };

            try
            {
                await _context.Feedbacks.AddAsync(feedback);
                var saveResult = await _context.SaveChangesAsync();
                return saveResult > 0 ? 1 : 0; // Check the result of SaveChangesAsync
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
                FullName = customerAccount.FullName,
                Phone = customerAccount.Phone,
                Status = customerAccount.Status
            };
        }

        public class CustomerAccountDTO
        {
            public int CustomerId { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? FullName { get; set; }
            public int? Status { get; set; }
        }
        public async Task<bool> VerifyConfirmationCodeAsync(string email, string code)
        {
            var customer = await _context.CustomersAccounts
                .FirstOrDefaultAsync(x => x.Email == email);

            if (customer == null || customer.ConfirmationCode != code || customer.CodeExpiry < DateTime.UtcNow)
            {
                return false; // Invalid code or expired
            }

            customer.Status = 1; // Active
            customer.ConfirmationCode = null; // Clear the code
            customer.CodeExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CustomerLoginResponse> LoginAsync(CustomerLoginDto dto)
        {
            var customer = await _context.CustomersAccounts
        .FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (customer == null || !VerifyPassword(dto.Password, customer.Password))
            {
                return null;
            }



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
        public async Task<bool> DeletePersonalAddressAsync(int personalAddressId)
        {
            try
            {
                // Find the personal address by ID
                var personalAddress = await _context.PersonalAddresses
                    .FirstOrDefaultAsync(pa => pa.AddressId == personalAddressId);

                // Check if it exists
                if (personalAddress == null)
                {
                    return false; // Address not found
                }

                // Remove the personal address
                _context.PersonalAddresses.Remove(personalAddress);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                // Log the error (implement logging mechanism as needed)
                Console.WriteLine($"Error deleting personal address: {ex.Message}");
                return false; // Deletion failed
            }
        }

    }
}
