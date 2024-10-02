using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface INewRepository
    {
        Task<List<GetNewDto>> GetAllNewsAsync();
        Task<GetNewDto?> GetNewsByIdAsync(int id);
        Task<bool> UpdateNewsStatusAsync(int id, int newStatus);
    }
}
