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
        private static readonly Dictionary<string, (Customer Customer, string Code, DateTime Expiry)> _pendingRegistrations = new();

        public CustomerRepository(Sep490G53Context context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<int> RegisterAsync(CustomerRegisterRequest cusRegReq)
        {
            var existedCus = await _context.Customers.FirstOrDefaultAsync(x => x.Email == cusRegReq.Email);
            if (existedCus != null) return 0;
            try
            {
                var confirmationCode = GenerateConfirmationCode();
                var subject = "Confirmation Code";
                var message = $"Your confirmation code is: {confirmationCode}";
                await _emailService.SendEmailAsync(cusRegReq.Email, subject, message);
                var temporaryCustomer = new Customer()
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

        private string GenerateConfirmationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<bool> VerifyConfirmationCodeAsync(string email, string code)
        {
            if (_pendingRegistrations.TryGetValue(email, out var storedData))
            {
                var (customer, confirmationCode, expiry) = storedData;
                if (confirmationCode == code && expiry > DateTime.UtcNow)
                {
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();
                    _pendingRegistrations.Remove(email);
                    return true;
                }
            }

            return false;
        }
        public async Task<CustomerLoginResponse> LoginAsync(CustomerLoginDto dto)
        {
            var customer = await _context.Customers
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


    }
}
