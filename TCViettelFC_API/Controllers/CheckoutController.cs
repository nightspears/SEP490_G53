using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos.CheckOut;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly Sep490G53Context _context;

        public CheckoutController(Sep490G53Context context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create Customer if not exists
              
                  var  customer = new Customer
                    {
                        AccountId = request.Customer.AccountId,
                        Email = request.Customer.Email,
                        Phone = request.Customer.Phone,
                        FullName= request.Customer.FullName,
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                

                // 2. Create Address
                var address = new Address
                {
                    CustomerId = customer.CustomerId,
                    City = request.Address.City,
                    CityName = request.Address.CityName,
                    District = request.Address.District,
                    DistrictName = request.Address.DistrictName,
                    Ward = request.Address.Ward,
                    WardName = request.Address.WardName,
                    DetailedAddress = request.Address.DetailedAddress,
                };
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                // 3. Create OrderProduct
                var orderProduct = new OrderProduct
                {
                    CustomerId = customer.CustomerId,
                    OrderCode = request.OrderProduct.OrderCode,
                    OrderDate = request.OrderProduct.OrderDate,
                    TotalPrice = request.OrderProduct.TotalPrice,
                    AddressId = address.AddressId,
                };
                _context.OrderProducts.Add(orderProduct);
                await _context.SaveChangesAsync();

                // 4. Create OrderProductDetails
                foreach (var detail in request.OrderProductDetails)
                {
                    var orderProductDetail = new OrderProductDetail
                    {
                        OrderProductId = orderProduct.Id,
                        ProductId = detail.ProductId,
                        PlayerId = detail.PlayerId,
                        CustomShirtNumber = detail.CustomShirtNumber,
                        CustomShirtName = detail.CustomShirtName,
                        Size = detail.Size,
                        Quantity = detail.Quantity,
                        Price = detail.Price,
                    };
                    _context.OrderProductDetails.Add(orderProductDetail);
                }
                await _context.SaveChangesAsync();

                // 5. Create Payment
                var payment = new Payment
                {
                    OrderProductId = orderProduct.Id,
                    TotalAmount = request.Payment.TotalAmount,
                    PaymentGateway = request.Payment.PaymentGateway,
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
                return Ok(new { Message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();
                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
