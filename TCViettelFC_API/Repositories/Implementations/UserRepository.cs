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
    public class UserRepository : IUserRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailService _emailService;
        public UserRepository(IEmailService emailService, Sep490G53Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = httpContextAccessor;
            _emailService = emailService;
        }
        public async Task AddUserAsync(UserCreateDto userCreateDto)
        {
            var user = new User
            {
                FullName = userCreateDto.FullName,
                Password = HashPassword(userCreateDto.Password),
                Email = userCreateDto.Email,
                Phone = userCreateDto.Phone,
                RoleId = userCreateDto.RoleId,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
        public bool ExistedUser(string email, string phoneNumber)
        {
            return _context.Users.Any(x => x.Email == email || x.Phone == phoneNumber);
        }
        public bool CheckExistedEmailAsync(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null) return false;
            return true;
        }
        public async Task<bool> SendConfirmationCodeAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return false;
            try
            {
                var confirmationCode = GenerateConfirmationCode();
                var subject = "Mã xác nhận tài khoản";
                var message = $"Mã xác nhận của bạn là: {confirmationCode}";
                await _emailService.SendEmailAsync(user.Email, subject, message);
                user.ConfirmationCode = confirmationCode;
                user.CodeExpiry = DateTime.UtcNow.AddMinutes(15);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> VerifyConfirmationCodeAsync(string email, string code)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null || user.ConfirmationCode != code || user.CodeExpiry < DateTime.UtcNow)
            {
                return false;
            }
            try
            {
                user.ConfirmationCode = null;
                user.CodeExpiry = null;
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }

        }
        public async Task<int> ResetPasswordAsync(string email, string newPass)
        {
            var cus = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (VerifyPassword(newPass, cus.Password)) return -1;
            if (cus == null) return 0;
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
        private string GenerateConfirmationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<int> AdminChangePasswordAsync(ChangePassRequest ch)
        {
            if (ch == null) return 0;
            var userId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null) return 0;
            if (!VerifyPassword(ch.OldPass, user.Password)) return -1;
            if (VerifyPassword(ch.NewPass, user.Password)) return -2;
            try
            {
                user.Password = HashPassword(ch.NewPass);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return 0;
            }

        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.Password))
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
                Subject = new ClaimsIdentity(
                [
                        new Claim("UserId", user.UserId.ToString()),
                            new Claim("RoleId",user.RoleId.ToString())
                    ]),
                Expires = DateTime.UtcNow.AddHours(6),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)

            };
            var jwtToken = jwtHandler.CreateToken(tokenDes);
            token = jwtHandler.WriteToken(jwtToken);
            var res = new LoginResponse()
            {
                Token = token,
                Email = user.Email,
                Phone = user.Phone,
                UserId = user.UserId,
                RoleId = user.RoleId,
                FullName = user.FullName,
                Status = user.Status
            };
            return res;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Delete failed", ex);
            }
        }

        public async Task<IEnumerable<Role>> GetRoleAsync()
        {
            return await _context.Roles
              .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
              .Include(u => u.Role).Where(x => x.RoleId == 1 || x.RoleId == 3)
              .ToListAsync();
        }

        public async Task<bool> IsValidRoleAsync(int? roleId)
        {
            if (roleId == null) return false;
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId);
        }

        public async Task UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            // Validate RoleId
            if (userDto.RoleId.HasValue && (userDto.RoleId < 0 || userDto.RoleId > 4))
            {
                throw new ArgumentException("Invalid RoleId.");
            }
            // Update user properties
            user.Email = userDto.Email ?? user.Email;
            user.Phone = userDto.Phone ?? user.Phone;
            user.FullName = userDto.FullName ?? user.FullName;
            user.RoleId = userDto.RoleId ?? user.RoleId;
            user.Status = userDto.Status ?? user.Status;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user: " + ex.Message);
            }
        }
    }
}
