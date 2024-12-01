using TCViettelFC_API.Dtos;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface INewRepository
    {
        IQueryable<GetNewDto> GetAllNewsAsQueryable();
        Task<List<GetNewDto>> GetAllNewsAsync();
        Task<GetNewDto?> GetNewsByIdAsync(int id);
        Task<bool> UpdateNewsStatusAsync(int id, int newStatus);

        Task<int> CreateNewsAsync(CreateNewDto newDto);
        Task<bool> UpdateNewsAsync(int id, UpdateNewDto newDto);
        Task<bool> DeleteNewsAsync(int id);
        Task<IEnumerable<GetNewDto>> GetAllActiveNews();

    }
}
