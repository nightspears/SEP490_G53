namespace TCViettelFC_Client.ApiServices
{
    public interface IApiHelper
    {
        Task<T> GetApiResponseAsync<T>(string endpoint, string token);
        Task<string> CreateApiResponseAsync<T>(string endpoint, T data);
        Task<string> DeleteApiResponseAsync(string endpoint);
        Task<string> UpdateApiResponseAsync<T>(string endpoint, T data);
    }
}
