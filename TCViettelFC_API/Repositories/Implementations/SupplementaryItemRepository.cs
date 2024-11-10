using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos.Supplementary;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class SupplementaryItemRepository: ISupplementaryItemRepository
    {
        private readonly Sep490G53Context _context;

        public SupplementaryItemRepository(Sep490G53Context context)
        {
            _context = context;

        }
        public async Task<IEnumerable<SupplementaryDto>> GetAllAsync()
        {
            var items = await _context.SupplementaryItems
                .Select(item => new SupplementaryDto
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Status = item.Status
                })
                .ToListAsync();

            return items;
        }


        public async Task<SupplementaryItem> GetByIdAsync(int itemId)
        {
            return await _context.SupplementaryItems.FindAsync(itemId);
        }

        public async Task<SupplementaryItem> CreateAsync(SupplementaryDto dto)
        {
            var item = new SupplementaryItem
            {
                ItemName = dto.ItemName,
                Price = dto.Price,
                Status = dto.Status
            };

            _context.SupplementaryItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(int id, SupplementaryDto dto)
        {
            var item = await _context.SupplementaryItems.FindAsync(id);
            if (item != null)
            {
                item.ItemName = dto.ItemName;
                item.Price = dto.Price;
                item.Status = dto.Status;

                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item not found"); // Hoặc xử lý khác phù hợp với logic ứng dụng của bạn
            }
        }

        public async Task DeleteAsync(int itemId)
        {
            var item = await _context.SupplementaryItems.FindAsync(itemId);
            if (item != null)
            {
                _context.SupplementaryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
