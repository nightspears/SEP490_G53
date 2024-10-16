using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class AreaRepository : IAreaRepository
    {
        private readonly Sep490G53Context _context;
        public AreaRepository(Sep490G53Context context)
        {
            _context = context;
        }
        public async Task<AreaDto?> GetAreaById(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return null;
            var result = new AreaDto()
            {
                Floor = area.Floor,
                Id = area.Id,
                Price = area.Price,
                Section = area.Section,
                Stands = area.Stands,
                Status = area.Status,
            };
            return result;
        }
    }
}
