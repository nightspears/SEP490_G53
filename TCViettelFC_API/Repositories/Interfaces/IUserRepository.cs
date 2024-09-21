using TCViettelFC_API.Dtos;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisterAsync(RegisterDto registerDto);
        bool ExistedUser(string username);
    }
}
