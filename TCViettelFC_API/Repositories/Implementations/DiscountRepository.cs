using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos.Discount;
using TCViettelFC_API.Dtos.Matches;
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

            //if (_discount.ValidUntil < DateTime.Now)
            //{
            //    throw new ArgumentException("valid_until must greater than today");
            //}
            //if (_discount.ValidFrom < DateTime.Now)
            //{
            //    throw new ArgumentException("valid_from must greater than today");
            //}

            //if (_discount.ValidFrom > _discount.ValidUntil)
            //{
            //    throw new ArgumentException("valid_from must less than valid_until");
            //}

            //if (string.IsNullOrEmpty(_discount.DiscountName))
            //{
            //    throw new ArgumentException("The system returns an error, no new Discount are added.");
            //}

            //if (string.IsNullOrEmpty(_discount.DiscountPercent.ToString()))
            //{
            //    throw new ArgumentException("The system returns an error, no new Discount are added.");
            //}
            //if (string.IsNullOrEmpty(_discount.ValidUntil.ToString()))
            //{
            //    throw new ArgumentException("The system returns an error, no new Discount are added.");
            //}
            //if (string.IsNullOrEmpty(_discount.ValidFrom.ToString()))
            //{
            //    throw new ArgumentException("The system returns an error, no new Discount are added.");
            //}



            try
            {
                Discount discount = new Discount();
                {
                    discount.DiscountName = _discount.DiscountName;
                    discount.DiscountPercent = _discount.DiscountPercent;
                    discount.Status = _discount.Status == null ? 2 : _discount.Status;
                    discount.ValidFrom = _discount.ValidFrom;
                    discount.ValidUntil = _discount.ValidUntil;

                };
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
            if (discount == null || discount.Status == 0) throw new KeyNotFoundException("Discount not found");

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
                _context.Database.ExecuteSqlRaw("EXEC UpdateDiscountStatus");

                List<Discount> discounts = new List<Discount>();
                discounts = await _context.Discounts.Where(x => x.Status != 0).ToListAsync();

                return discounts;
            }
            catch (Exception ex)
            {
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
        public async Task UpdateDiscountAsync(int id, DiscountDto _discount)
        {

            if (_discount.ValidUntil < DateTime.Now)
            {
                throw new ArgumentException("valid_until must greater than today");
            }
            if (_discount.ValidFrom < DateTime.Now)
            {
                throw new ArgumentException("valid_from must greater than today");
            }

            if (_discount.ValidFrom > _discount.ValidUntil)
            {
                throw new ArgumentException("valid_from must less than valid_until");
            }

            if (string.IsNullOrEmpty(_discount.DiscountName))
            {
                throw new ArgumentException("The system returns an error, no new Discount are added.");
            }

            if (string.IsNullOrEmpty(_discount.DiscountPercent.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new Discount are added.");
            }
            if (string.IsNullOrEmpty(_discount.ValidUntil.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new Discount are added.");
            }
            if (string.IsNullOrEmpty(_discount.ValidFrom.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new Discount are added.");
            }

            try
            {
                var discount = await _context.Discounts.FindAsync(id);
                if (discount == null || discount.Status == 0)
                {
                    throw new Exception("Discount not found");
                }

                // Update Season properties
                discount.DiscountName = _discount.DiscountName ?? discount.DiscountName;
                discount.Status = _discount.Status ?? discount.Status;
                discount.ValidFrom = _discount.ValidFrom ?? discount.ValidFrom;
                discount.ValidUntil = _discount.ValidUntil ?? discount.ValidUntil;
                discount.DiscountPercent = _discount.DiscountPercent ?? discount.DiscountPercent;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Season: " + ex.Message);
            }
        }
    }
}
