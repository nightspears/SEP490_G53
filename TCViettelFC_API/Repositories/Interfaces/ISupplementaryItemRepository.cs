using TCViettelFC_API.Dtos.Supplementary;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ISupplementaryItemRepository
    {
        Task<IEnumerable<SupplementaryDto>> GetAllAsync();
        Task<SupplementaryItem> GetByIdAsync(int itemId);
        Task<SupplementaryItem> CreateAsync(SupplementaryDto dto);
        Task UpdateAsync(int id, SupplementaryDto dto);
        Task DeleteAsync(int itemId);
    }
}
