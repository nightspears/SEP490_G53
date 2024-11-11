using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Product;
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
        Task<List<ProductResponse>> GetSanPhamAsync(int id);
        Task<JsonResult> GetSanPhamByIdAsync(int id);
        Task<JsonResult> GetLienQuanProductAsync(List<int> lstID);


    }
}
