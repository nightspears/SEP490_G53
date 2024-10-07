using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductResponse>> GetProductAsync();

        Task AddProductAsync(ProductDto pro);
        Task<JsonResult> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, ProductDto pro);
        Task DeleteProductAsync(int id);
        Task<JsonResult> GetDataJsonAsync();


    }
}
