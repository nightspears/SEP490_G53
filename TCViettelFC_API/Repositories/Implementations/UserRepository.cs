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
    public class UserRepository : IUserRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserRepository(Sep490G53Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = httpContextAccessor;
        }
        public async Task AddUserAsync(UserCreateDto userCreateDto)
        {
            var user = new User
            {
                FullName = userCreateDto.FullName,
                Password = userCreateDto.Password,
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
        //public bool ExistedUser(string username, string phoneNumber)
        //{
        //    return _context.Users.Any(x => x.UserName == username || x.Phone == phoneNumber);
        //}
        //public async Task<int> RegisterAsync(RegisterDto registerDto)
        //{
        //    var user = new User
        //    {
        //        UserName = registerDto.UserName,
        //        Password = registerDto.Password,
        //        Email = registerDto.Email,
        //        Phone = registerDto.PhoneNumber,
        //        RoleId = 1,
        //        Status = 1,
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    try
        //    {
        //        await _context.Users.AddAsync(user);
        //        await _context.SaveChangesAsync();
        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}
        //public async Task<string> LoginAsync(LoginDto loginDto)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == loginDto.PhoneNumber && x.Password == loginDto.Password);
        //    if (user != null)
        //    {
        //        string token = string.Empty;
        //        var issuer = _configuration["JwtConfig:Issuer"];
        //        var audience = _configuration["JwtConfig:Audience"];
        //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
        //        var jwtHandler = new JwtSecurityTokenHandler();
        //        var tokenDes = new SecurityTokenDescriptor()
        //        {
        //            Subject = new ClaimsIdentity(
        //            [
        //                    new Claim("UserId", user.UserId.ToString()),
        //                    new Claim("RoleId",user.RoleId.ToString())
        //                ]),
        //            Expires = DateTime.UtcNow.AddHours(6),
        //            Audience = audience,
        //            Issuer = issuer,
        //            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)

        //        };
        //        var jwtToken = jwtHandler.CreateToken(tokenDes);
        //        token = jwtHandler.WriteToken(jwtToken);
        //        return token;
        //    }
        //    return "";
        //}

        public async Task<int> AdminChangePasswordAsync(ChangePassRequest ch)
        {
            if (ch == null) return 0;
            var userId = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (!user.Password.Equals(ch.OldPass)) return 0;
            if (user == null || user.Password.Equals(ch.NewPass)) return 0;
            try
            {
                user.Password = ch.NewPass;
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == loginDto.Phone && x.Password == loginDto.Password);
            if (user != null)
            {
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
                    FullName = user.FullName
                };
                return res;
            }
            return null;
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
              .Include(u => u.Role).Where(x => x.RoleId == 1)
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

            // Update user properties
            user.Email = userDto.Email ?? user.Email;
            user.Phone = userDto.Phone ?? user.Phone;
            user.FullName = userDto.FullName ?? user.FullName;
            user.RoleId = userDto.RoleId ?? user.RoleId;
            user.Status = userDto.Status ?? user.Status;
            user.CreatedAt = userDto.CreatedAt ?? user.CreatedAt; // Only update if provided

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
