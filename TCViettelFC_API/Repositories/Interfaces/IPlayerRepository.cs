using TCViettelFC_API.Dtos;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<PlayerDto>> ListAllPlayerAsync();
        Task<PlayerDto> GetPlayerByIdAsync(int id);
        Task<PlayerDto> AddPlayerAsync(PlayerDto player);
        Task<PlayerDto> UpdatePlayerAsync(PlayerDto player);
        Task<PlayerDto> DeletePlayerAsync(int id);

    }
}
