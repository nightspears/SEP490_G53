using TCViettelFC_API.Dtos;
using static TCViettelFC_API.Repositories.Implementations.CustomerRepository;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerLoginResponse> LoginAsync(CustomerLoginDto customerLoginDto);
        Task<string> RegisterAsync(CustomerRegisterRequest customerRegisterRequest);
        Task<bool> VerifyConfirmationCodeAsync(string email, string code);
        Task<ProfileDto?> GetCustomerProfile();
        Task<int> UpdateCustomerProfile(ProfileDto profileDto);
        Task<List<TicketOrderHistoryDto>> GetTicketOrderHistory();

        Task<int> PostFeedback(FeedbackPostDto feedbackDto);

        Task<CustomerAccountDTO?> GetCustomerByAccountIdAsync(int accountId);
        Task<List<PersonalAddressDTO>> GetPersonalAddressesByCustomerIdAsync(int customerId);


        Task<bool> InsertPersonalAddressAsync(PersonalAddressCreateDto personalAddressDto);

        Task<bool> DeletePersonalAddressAsync(int personalAddressId);
        Task<int> CheckExistedCustomerEmail(string email);
        Task<bool> SendConfirmationCodeAsync(string email);
        Task<int> ChangePasswordAsync(ChangePassRequest changePassRequest);
        Task<int> ResetPasswordAsync(string email, string newPass);

    }
}
