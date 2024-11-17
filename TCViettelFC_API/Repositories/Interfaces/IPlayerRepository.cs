using TCViettelFC_API.Dtos;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<ShowPlayerDtos>> ListAllPlayerAsync();
        Task<List<ShowPlayerDtos>> ListAllPlayerActiveAsync();
        Task<ShowPlayerDtos> GetPlayerByIdAsync(int id);
        Task<ShowPlayerDtos> AddPlayerAsync(PlayerDto playerDto);
        Task<ShowPlayerDtos> UpdatePlayerAsync(int id, PlayerDto playerDto);
        Task<ShowPlayerDtos> DeletePlayerAsync(int id);
    }
}
