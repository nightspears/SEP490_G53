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
        Task<UserProfileDto> GetUserProfile();
        Task<bool> UpdateUserProfile(UserProfileDto model);
        bool CheckExistedEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<int> ResetPasswordAsync(string email, string newPass);
        Task<IEnumerable<Role>> GetRoleAsync();
        Task AddUserAsync(UserCreateDto userCreateDto);
        Task<bool> SendConfirmationCodeAsync(string email);
        Task<bool> VerifyConfirmationCodeAsync(string email, string code);
        Task UpdateUserAsync(int id, UserUpdateDto userDto);
        Task DeleteUserAsync(int id);
        Task<bool> IsValidRoleAsync(int? roleId);
        bool ExistedUser(string username, string phoneNumber);

    }
}
