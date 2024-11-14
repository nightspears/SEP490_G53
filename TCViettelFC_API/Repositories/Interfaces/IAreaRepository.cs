using TCViettelFC_API.Dtos;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IAreaRepository
    {
        Task<AreaDto?> GetAreaById(int id);

        Task<List<MatchAreaTicketDto>> GetMatchAreaTicketsByMatchIdAsync(int matchId);
    }
}
