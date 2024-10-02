using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        //Task<int> RegisterAsync(RegisterDto registerDto);
        //bool ExistedUser(string username, string phoneNumber);
        //Task<AdminLoginResponse> LoginAsync(LoginDto loginDto);
        Task<LoginResponse> LoginAsync(LoginDto loginDto);
        Task<int> AdminChangePasswordAsync(ChangePassRequest ch);


        Task<IEnumerable<User>> GetUsersAsync();
        Task<IEnumerable<Role>> GetRoleAsync();
        Task AddUserAsync(UserCreateDto userCreateDto);

        Task UpdateUserAsync(int id, UserUpdateDto userDto);
        Task DeleteUserAsync(int id);
        Task<bool> IsValidRoleAsync(int? roleId);
        bool ExistedUser(string username, string phoneNumber);

    }
}
