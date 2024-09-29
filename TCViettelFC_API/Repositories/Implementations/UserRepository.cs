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
        public async Task<AdminLoginResponse> AdminLoginAsync(AdminLoginDto loginDto)
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
                var res = new AdminLoginResponse()
                {
                    Token = token,
                    Email = user.Email,
                    Phone = user.Phone,
                    UserId = user.UserId,
                    FullName = user.FullName
                };
                return res;
            }
            return null;
        }
    }
}
