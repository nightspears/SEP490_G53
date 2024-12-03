using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class NewRepository : INewRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICloudinarySetting _cloudinary;
        public NewRepository(Sep490G53Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ICloudinarySetting cloudinary)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = httpContextAccessor;
            _cloudinary = cloudinary;
        }
        public IQueryable<GetNewDto> GetAllNewsAsQueryable()
        {
            var news = _context.News.Include(x => x.Creator).Include(x => x.NewsCategory).Where(x => x.Status == 1).Select(n => new GetNewDto
            {
                Id = n.Id,
                CreatorId = n.Creator.FullName,
                NewsCategory = n.NewsCategory.CategoryName,
                Title = n.Title,
                Content = n.Content,
                Image = n.Image,
                CreatedAt = n.CreatedAt,
                Status = n.Status,
                NewsCategoryId = n.NewsCategoryId
                
            }).AsQueryable();

            return news;

        }
        public async Task<List<GetNewDto>> GetAllNewsAsync()
        {
            var news = await _context.News.Include(x => x.Creator).Include(x => x.NewsCategory)
                .Select(n => new GetNewDto
                {
                    Id = n.Id,
                    CreatorId = n.Creator.FullName,
                    NewsCategory = n.NewsCategory.CategoryName,
                    Title = n.Title,
                    Content = n.Content,
                    Image = n.Image,
                    CreatedAt = n.CreatedAt,
                    Status = n.Status,
                    NewsCategoryId = n.NewsCategoryId
                })
                .ToListAsync();

            return news;

        }
        // Method to get news by ID and convert to GetNewDto
        public async Task<GetNewDto?> GetNewsByIdAsync(int id)
        {
            var news = await _context.News
                .Include(x => x.Creator)
                .Include(x => x.NewsCategory)
                .Where(n => n.Id == id)
                .Select(n => new GetNewDto
                {
                    Id = n.Id,
                    CreatorId = n.Creator.FullName,
                    NewsCategory = n.NewsCategory.CategoryName,
                    Title = n.Title,
                    Content = n.Content,
                    Image = n.Image,
                    CreatedAt = n.CreatedAt,
                    Status = n.Status,
                    NewsCategoryId = n.NewsCategoryId
                })
                .FirstOrDefaultAsync();

            return news;
        }
        public async Task<bool> UpdateNewsStatusAsync(int id, int newStatus)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return false;
            }
            news.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateNewsAsync(CreateNewDto newDto)
        {
            using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    // Kiểm tra các giá trị null cần thiết
                    if (newDto.NewsCategoryId == null)
                    {
                        throw new InvalidOperationException("NewsCategoryId cannot be null.");
                    }
                    if (newDto.CreatorId == null)
                    {
                        throw new InvalidOperationException("CreatorId cannot be null.");
                    }
                    if (newDto.Status == null)
                    {
                        throw new InvalidOperationException("Status cannot be null.");
                    }
                    // Kiểm tra giá trị của Title, nếu null hoặc rỗng thì ném ngoại lệ
                    if (string.IsNullOrWhiteSpace(newDto.Title))
                    {
                        throw new InvalidOperationException("Title cannot be null or empty.");
                    }
                    // Kiểm tra giá trị của CreatedAt
                    if (newDto.CreatedAt == null)
                    {
                        newDto.CreatedAt = DateTime.UtcNow;
                    }
                    if (newDto.CreatedAt == null || newDto.CreatedAt == default(DateTime))
                    {
                        throw new InvalidOperationException("CreatedAt cannot be null or invalid format.");
                    }
                    // Upload ảnh lên Cloudinary nếu có
                    var uploadResult = new ImageUploadResult();
                    if (newDto.Image != null && newDto.Image.Length > 0)
                    {
                        uploadResult = _cloudinary.CloudinaryUpload(newDto.Image);
                    }

                    // Lưu thông tin vào cơ sở dữ liệu
                    var news = new News
                    {
                        CreatorId = newDto.CreatorId,
                        NewsCategoryId = newDto.NewsCategoryId,
                        Title = newDto.Title,
                        Content = newDto.Content,
                        Image = uploadResult?.SecureUrl?.ToString() ?? "/image/default_image.jpg", // Sử dụng ảnh mặc định nếu không upload được
                        CreatedAt = DateTime.UtcNow,
                        Status = newDto.Status
                    };

                    _context.News.Add(news);
                    await _context.SaveChangesAsync();

                    await dbContextTransaction.CommitAsync();
                    return news.Id;
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
        }




        public async Task<bool> UpdateNewsAsync(int id, UpdateNewDto newDto)
        {
            using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tìm bản ghi cần cập nhật
                    var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
                    if (news == null)
                    {
                        return false;
                    }

                    // Cập nhật các thông tin cơ bản
                    news.CreatorId = newDto.CreatorId ?? news.CreatorId;
                    news.NewsCategoryId = newDto.NewsCategoryId ?? news.NewsCategoryId;
                    news.Title = newDto.Title ?? news.Title;
                    news.Content = newDto.Content ?? news.Content;

                    // Xử lý ảnh mới nếu có
                    if (newDto.Image != null && newDto.Image.Length > 0)
                    {
                        var uploadResult = _cloudinary.CloudinaryUpload(newDto.Image);
                        news.Image = uploadResult?.SecureUrl?.ToString() ?? news.Image; // Giữ nguyên ảnh cũ nếu upload thất bại
                    }

                    // Cập nhật thời gian chỉnh sửa
                    news.CreatedAt = DateTime.UtcNow;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                    return true;
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
        }



        public async Task<bool> DeleteNewsAsync(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return false;
            }
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
