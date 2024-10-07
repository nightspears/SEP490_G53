using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<ProductCategory>> GetCateAsync();

        Task AddCateAsync(CategoryDto cate);
        Task<ProductCategory> GetCateByIdAsync(int id);
        Task UpdateCateAsync(int id, CategoryDto cate);
        Task DeleteCateAsync(int id);

    }
}
