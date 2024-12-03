using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.CheckOut;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly Sep490G53Context _context;
        private readonly ITicketUtilRepository _ticketUtilRepository;
        public CheckoutController(Sep490G53Context context, ITicketUtilRepository ticketUtilRepository)
        {
            _context = context;
            _ticketUtilRepository = ticketUtilRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request.Customer == null || string.IsNullOrEmpty(request.Customer.FullName) || string.IsNullOrEmpty(request.Customer.Email))
            {
                return BadRequest(new { Message = "Customer data is required." });
            }

            // Validate Address data
            if (request.Address == null || string.IsNullOrEmpty(request.Address.City) || string.IsNullOrEmpty(request.Address.District) || string.IsNullOrEmpty(request.Address.Ward))
            {
                return BadRequest(new { Message = "Address data is required." });
            }
            if (request.OrderProduct == null || string.IsNullOrEmpty(request.OrderProduct.OrderCode) || string.IsNullOrEmpty(request.OrderProduct.ShipmentFee.ToString()) || string.IsNullOrEmpty(request.OrderProduct.OrderDate.ToString()))
            {
                return BadRequest(new { Message = "Order product data is required." });
            }

            if (request.OrderProductDetails == null || !request.OrderProductDetails.Any())
            {
                return BadRequest(new { Message = "Order product details are required." });
            }

            if (string.IsNullOrEmpty(request.Payment?.PaymentGateway))
            {
                return BadRequest(new { Message = "Payment gateway is required." });
            }
            if (request.Payment?.TotalAmount < 0 || request.OrderProduct?.TotalPrice < 0)
            {
                return BadRequest(new { Message = "Total price and payment amount cannot be negative." });
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create Customer if not exists

                var customer = new Customer
                {
                    AccountId = request.Customer.AccountId,
                    Email = request.Customer.Email,
                    Phone = request.Customer.Phone,
                    FullName = request.Customer.FullName,
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
                    ShipmentFee = request.OrderProduct.ShipmentFee,
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
                _ticketUtilRepository.SendOrderConfirmationEmailAsync(request);
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
