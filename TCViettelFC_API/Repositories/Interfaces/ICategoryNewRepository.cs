using TCViettelFC_API.Dtos;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICategoryNewRepository
    {
        Task<List<GetNewCategoryDto>> GetAllCategoryNewAsync();
    }
}
