using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class CategoryNewRepository : ICategoryNewRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public CategoryNewRepository(Sep490G53Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = httpContextAccessor;
        }
        public async Task<List<GetNewCategoryDto>> GetAllCategoryNewAsync()
        {
            var categories = await _context.NewsCategories.Include(c => c.Creator)
                .Select(c => new GetNewCategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    CreatorName = c.Creator != null ? c.Creator.FullName : "Unknown",
                    Status = c.Status ?? 0
                })
                .ToListAsync();

            return categories;
        }
    }
}
