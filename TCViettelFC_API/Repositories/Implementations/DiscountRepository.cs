using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCViettelFC_API.Dtos.Discount;
using TCViettelFC_API.Dtos.Season;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class DiscoutRepository : IDiscountRepository
    {
        private readonly Sep490G53Context _context;

        public DiscoutRepository(Sep490G53Context context)
        {
            _context = context;

        }
        public async Task AddDiscountAsync(DiscountDto _discount)
        {

            Discount discount = new Discount();
            {
                discount.DiscountName = _discount.DiscountName;
                discount.DiscountPercent = _discount.DiscountPercent;
                discount.Status = _discount.Status;
                discount.ValidFrom = _discount.ValidFrom;
                discount.ValidUntil = _discount.ValidUntil;
                  
            };
          
            try
            {
                await _context.Discounts.AddAsync(discount);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Discount not found");
            }
        }
        public async Task DeleteDiscountAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null|| discount.Status == 0) throw new KeyNotFoundException("Discount not found");

            try
            {
                discount.Status = 0;
                await _context.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Delete failed", ex);
            }
        }
      

        public async Task<List<Discount>> GetDiscountAsync()
        {
            try
            {
                List<Discount> discounts = new List<Discount>();
                discounts = await _context.Discounts.Where(x => x.Status != 0).ToListAsync();

                return discounts;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
           
        }
        public async Task<Discount> GetDiscountByIdAsync(int id)
        {
            Discount discount = new Discount();
            discount = await _context.Discounts.FirstOrDefaultAsync(x => x.DiscountId == id && x.Status == 1);
              
            if (discount == null)
            {
                throw new Exception("Discount not found");
            }
            else
            {
                return discount;
            }
        }
        public async Task UpdateDiscountAsync(int id, DiscountDto discountDto)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(id);
                if (discount == null || discount.Status == 0)
                {
                    throw new Exception("Discount not found");
                }

                // Update Season properties
                discount.DiscountName = discountDto.DiscountName ?? discount.DiscountName;
                discount.Status = discountDto.Status ?? discount.Status ;
                discount.ValidFrom = discountDto.ValidFrom ?? discount.ValidFrom;
                discount.ValidUntil = discountDto.ValidUntil ?? discount.ValidUntil;
                discount.DiscountPercent = discountDto.DiscountPercent ?? discount.DiscountPercent;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Season: " + ex.Message);
            }
        }
    }
}
