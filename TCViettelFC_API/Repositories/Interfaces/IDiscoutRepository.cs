using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Discount;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<List<Discount>> GetDiscountAsync();

        Task AddDiscountAsync(DiscountDto discount);
        Task<Discount> GetDiscountByIdAsync(int id);
        Task UpdateDiscountAsync(int id, DiscountDto discount);
        Task DeleteDiscountAsync(int id);

    }
}
