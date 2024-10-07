using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TCViettelFC_Client.Services;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Services
{
    public class FeedbackService : BaseService
    {
        private const string FeedbackEndpoint = "api/Feedback/GetFeedbacks";
        private const string UpdateFeedbackEndpoint = "api/Feedback/";

        public FeedbackService(HttpClient httpClient) : base(httpClient)
        {
        }

 
        public async Task<IEnumerable<FeedbackResponse>> GetFeedbacksAsync(string token)
        {
            var response = await GetAsync("Feedback/GetFeedbacks", token); // Use relative path from Admin

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Ensure correct deserialization settings
                var feedbacks = JsonSerializer.Deserialize<IEnumerable<FeedbackResponse>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Ensure camelCase handling if needed
                    WriteIndented = true
                });

                return feedbacks ?? Enumerable.Empty<FeedbackResponse>();
            }

            return Enumerable.Empty<FeedbackResponse>();
        }
        public async Task<bool> ApproveFeedbackAsync(int feedbackId, int responderId, string token)
        {
            // Set the Authorization header with the provided token
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Create the data to send with the request
            var updateData = new
            {
                ResponderId = responderId,
                Status = 1 // Assuming '1' indicates approved status
            };

            // Send PUT request to update feedback using relative path
            var response = await _httpClient.PutAsJsonAsync($"Feedback/{feedbackId}", updateData);

            if (response.IsSuccessStatusCode)
            {
                return true; // Approval was successful
            }
            else
            {
                // Log error or throw exception if necessary
                // e.g., Log error, check response.ReasonPhrase or StatusCode for more details
                return false;
            }
        }


    }
}
