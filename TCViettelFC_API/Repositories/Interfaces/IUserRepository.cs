using TCViettelFC_API.Dtos;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        //Task<int> RegisterAsync(RegisterDto registerDto);
        //bool ExistedUser(string username, string phoneNumber);
        //Task<AdminLoginResponse> LoginAsync(LoginDto loginDto);
        Task<AdminLoginResponse> AdminLoginAsync(AdminLoginDto loginDto);
        Task<int> AdminChangePasswordAsync(ChangePassRequest ch);

    }
}
