using System.Text.Json;
using TCViettelFC_Client.Services;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Services
{
    public class OrderService : BaseService
    {
        public OrderService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<IEnumerable<OrderProductDto>> GetAllOrderProductsAsync(string token = null)
        {
            var endpoint = "Order/getproductorders"; // Endpoint for getting all order products
            var response = await GetAsync(endpoint, token);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<OrderProductDto>>(content);
            }
            else
            {
                // Handle error or throw exception based on your requirements
                throw new HttpRequestException($"Error retrieving orders: {response.ReasonPhrase}");
            }
        }


        public async Task<OrderDetailDto> GetOrderDetailsAsync(int orderId, string token = null)
        {
            var endpoint = $"Order/getorderdetails/{orderId}";
            var response = await GetAsync(endpoint, token);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Set up JSON serialization options with camelCase
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                return JsonSerializer.Deserialize<OrderDetailDto>(content, options);
            }
            else
            {
                throw new HttpRequestException($"Error retrieving order details: {response.ReasonPhrase}");
            }
        }

    }
}
