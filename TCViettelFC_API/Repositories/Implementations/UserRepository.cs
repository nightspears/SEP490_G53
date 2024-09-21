using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly Sep490G53Context _context;
        public UserRepository(Sep490G53Context context)
        {
            _context = context;
        }
        public bool ExistedUser(string username)
        {
            return _context.Users.Any(x => x.UserName == username);
        }
        public async Task<int> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.UserName,
                Password = registerDto.Password,
                Email = registerDto.Email,
                Phone = registerDto.PhoneNumber,
                RoleId = 1,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
