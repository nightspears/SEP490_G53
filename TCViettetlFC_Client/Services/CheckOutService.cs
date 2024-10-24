using TCViettelFC_Client.Services;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Services
{
    public class CheckOutService : BaseService
    {
        public CheckOutService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<string> CreateOrderAsync(CreateOrderRequest createOrderRequest, string token = null)
        {
            var response = await PostAsync("Checkout/create", createOrderRequest, token);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result; // You can deserialize this if needed.
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return $"Error: {errorResponse}";
            }
        }
    }
}
