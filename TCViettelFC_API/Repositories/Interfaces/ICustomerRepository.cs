using TCViettelFC_API.Dtos;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerLoginResponse> LoginAsync(CustomerLoginDto customerLoginDto);
        Task<int> RegisterAsync(CustomerRegisterRequest customerRegisterRequest);
        Task<bool> VerifyConfirmationCodeAsync(string email, string code);
        Task<ProfileDto?> GetCustomerProfile();
        Task<int> UpdateCustomerProfile(ProfileDto profileDto);

    }
}
