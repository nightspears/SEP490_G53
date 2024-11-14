using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class CheckoutRepository: ICheckoutRepository
    {
        private readonly Sep490G53Context _context;

        public CheckoutRepository(Sep490G53Context context)
        {
            _context = context;
        }

        public async Task<bool> InsertCheckoutDataAsync(
         Customer customer,
         Address address,
         OrderProduct orderProduct,
         List<OrderProductDetail> orderProductDetails,
         Payment payment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Insert or update customer
                if (customer.CustomerId == 0)
                {
                    _context.Customers.Add(customer);
                }
                else
                {
                    _context.Customers.Update(customer);
                }

                await _context.SaveChangesAsync();

                // Set the CustomerId in Address if necessary
                address.CustomerId = customer.CustomerId;
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                // Set the CustomerId in OrderProduct if necessary
                orderProduct.CustomerId = customer.CustomerId;
                _context.OrderProducts.Add(orderProduct);
                await _context.SaveChangesAsync();

                // Set the OrderProductId in OrderProductDetails and add each detail
                foreach (var detail in orderProductDetails)
                {
                    detail.OrderProductId = orderProduct.Id;
                    _context.OrderProductDetails.Add(detail);
                }
                await _context.SaveChangesAsync();

                // Set the OrderProductId in Payment
                payment.OrderProductId = orderProduct.Id;
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                Console.WriteLine($"Error during checkout process: {ex.Message}");

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task SaveAddressAsync(Address address)
        {
            if (address.AddressId == 0)
            {
                // Add new address
                _context.Addresses.Add(address);
            }
            else
            {
                // Update existing address
                _context.Addresses.Update(address);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveCustomerAsync(Customer customer)
        {
            if (customer.CustomerId == 0)
            {
                // Add new customer
                _context.Customers.Add(customer);
            }
            else
            {
                // Update existing customer
                _context.Customers.Update(customer);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveOrderProductAsync(OrderProduct orderProduct)
        {
            if (orderProduct.Id == 0)
            {
                // Add new order product
                _context.OrderProducts.Add(orderProduct);
            }
            else
            {
                // Update existing order product
                _context.OrderProducts.Update(orderProduct);
            }

            await _context.SaveChangesAsync();
        }

    }

}
