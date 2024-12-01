using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos.Supplementary;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class SupplementaryItemRepository: ISupplementaryItemRepository
    {

        private readonly ICloudinarySetting _cloudinary;

        private readonly Sep490G53Context _context;

        public SupplementaryItemRepository(Sep490G53Context context, ICloudinarySetting cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;


        }
        public async Task<IEnumerable<SupplementaryRespone>> GetAllAsync()
        {
            var items = await _context.SupplementaryItems.Where(x => x.Status != 0)
                .Select(item => new SupplementaryRespone
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Status = item.Status,
                    Image = item.ImageUrl
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
                Status = dto.Status,


            };
            if (dto.Imageurl != null && dto.Imageurl.Length > 0)
            {
                ImageUploadResult res = _cloudinary.CloudinaryUpload(dto.Imageurl);
                item.ImageUrl = res.SecureUrl.ToString();
            }
            else
            {
                item.ImageUrl = "/image/imagelogo/ImageFail.jpg";
            }

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

                if (dto.Imageurl != null && dto.Imageurl.Length > 0)
                {
                    ImageUploadResult res = _cloudinary.CloudinaryUpload(dto.Imageurl);
                    item.ImageUrl = res.SecureUrl.ToString();
                }
              
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item not found"); 
            }
        }

        public async Task DeleteAsync(int itemId)
        {
            var item = await _context.SupplementaryItems.FindAsync(itemId);
            if (item != null)
            {
                item.Status = 0;
                await _context.SaveChangesAsync();
            }
        }

        public void UpdateStatus(int status, int id)
        {
            var product = _context.SupplementaryItems.Find(id);
            product.Status = status;
            _context.SaveChanges();
        }
    }
}
