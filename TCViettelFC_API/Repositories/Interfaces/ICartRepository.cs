using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICartRepository
    {
        void AddProductToCart(int productId, int quantity);
        void UpdateProductToCart(int productId, int quantity);

    }
}
