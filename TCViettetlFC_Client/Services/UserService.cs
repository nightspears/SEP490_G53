using System.Net.Http.Headers;
using System.Text.Json;
using TCViettetlFC_Client.Models;

namespace TCViettelFC_Client.Services
{
    public class UserService : BaseService
    {
        public UserService(HttpClient httpClient)
            : base(httpClient) // Base URL is now provided by the HttpClient factory
        {
        }

        public async Task<IEnumerable<UserViewModel>> GetUsersAsync(string token)
        {
            var response = await GetAsync("Admin/GetUsers", token); // Use relative path from Admin

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Ensure correct deserialization settings
                var users = JsonSerializer.Deserialize<IEnumerable<UserViewModel>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Ensure camelCase handling if needed
                    WriteIndented = true
                });

                return users ?? Enumerable.Empty<UserViewModel>();
            }

            return Enumerable.Empty<UserViewModel>();
        }

        // Fetch roles
        public async Task<IEnumerable<RoleViewModel>> GetRolesAsync(string token = null)
        {
            var response = await GetAsync("Admin/GetRoles", token);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var roles = JsonSerializer.Deserialize<IEnumerable<RoleViewModel>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Ensure camelCase handling if needed
                    WriteIndented = true
                });

                return roles ?? Enumerable.Empty<RoleViewModel>();
            }

            return Enumerable.Empty<RoleViewModel>();
        }

        // Add a new user
        public async Task<string> AddUserAsync(UserCreateDto userCreateDto, string token)
        {
            // Serialize the userCreateDto to JSON
            var jsonContent = new StringContent(JsonSerializer.Serialize(userCreateDto), System.Text.Encoding.UTF8, "application/json");

            // Add authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send POST request to add user
            var response = await _httpClient.PostAsync("Admin/AddUser", jsonContent);

            // Handle response
            if (response.IsSuccessStatusCode)
            {
                return "Tạo mới người dùng thành công";
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return $"Error: {errorResponse}"; // Return the error message from the API
            }
        }
        // **Update an existing user**
        public async Task<string> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto, string token)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(userUpdateDto), System.Text.Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsync($"Admin/UpdateUser/{userId}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return "Cập nhật người dùng thành công";
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return $"Error: {errorResponse}";
            }
        }

        // **Delete an existing user**
        public async Task<string> DeleteUserAsync(int userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"Admin/DeleteUser/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return "Xóa người dùng thành công";
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return $"Error: {errorResponse}";
            }
        }
    }
}
