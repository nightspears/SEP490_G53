using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using static TCViettelFC_API.Repositories.Implementations.CustomerRepository;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerLoginResponse> LoginAsync(CustomerLoginDto customerLoginDto);
        Task<int> RegisterAsync(CustomerRegisterRequest customerRegisterRequest);
        Task<bool> VerifyConfirmationCodeAsync(string email, string code);
        Task<ProfileDto?> GetCustomerProfile();
        Task<int> UpdateCustomerProfile(ProfileDto profileDto);
		Task<CustomerAccountDTO?> GetCustomerByAccountIdAsync(int accountId);
        Task<List<PersonalAddressDTO>> GetPersonalAddressesByCustomerIdAsync(int customerId);

		Task<bool> InsertPersonalAddressAsync(PersonalAddressCreateDto personalAddressDto);
	}
}
