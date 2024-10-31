using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos.Order;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Sep490G53Context _context;
        public OrderRepository(Sep490G53Context context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TicketOrdersDto>> GetAllTicketOrders()
        {
            var orders = await _context.TicketOrders.Include(x => x.Customer).ToListAsync();
            var listresults = new List<TicketOrdersDto>();
            foreach (var order in orders)
            {
                var dto = new TicketOrdersDto()
                {
                    CustomerEmail = order.Customer!.Email,
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                };
                listresults.Add(dto);
            }
            return listresults;
        }
        public async Task<IEnumerable<OrderedSuppItemDto>> GetOrderedSuppItemByTicketOrderId(int id)
        {
            var orders = await _context.OrderedSuppItems.Include(x => x.Item).Where(x => x.OrderId == id).ToListAsync();
            var results = new List<OrderedSuppItemDto>();
            foreach (var order in orders)
            {
                var dto = new OrderedSuppItemDto()
                {
                    Id = order.Id,
                    ItemName = order.Item!.ItemName,
                    OrderId = order.OrderId,
                    Price = order.Price,
                    Quantity = order.Quantity
                };
                results.Add(dto);
            }
            return results;
        }
        public async Task<IEnumerable<OrderedTicketDto>> GetOrderedTicketByTicketOrderId(int id)
        {
            var orders = await _context.OrderedTickets.Where(x => x.OrderId == id).ToListAsync();
            var results = new List<OrderedTicketDto>();
            foreach (var order in orders)
            {
                var dto = new OrderedTicketDto()
                {
                    Id = order.Id,
                    AreaId = order.AreaId,
                    MatchId = order.MatchId,
                    OrderId = order.OrderId,
                    Price = order.Price,
                    Status = order.Status
                };
                results.Add(dto);
            }
            return results;
        }

        public async Task<IEnumerable<OrderProductDto>> GetAllOrderProductsAsync()
        {
            return await _context.OrderProducts
                .Select(op => new OrderProductDto
                {
                    Id = op.Id,
                    CustomerId = op.CustomerId,
                    OrderCode = op.OrderCode,
                    OrderDate = op.OrderDate,
                    TotalPrice = op.TotalPrice,
                    AddressId = op.AddressId,
                    Note = op.Note,
                    Status = op.Status,
                    Email = op.Customer.Email,  // Including Email
                    Phone = op.Customer.Phone,
                    FullName = op.Customer.FullName// Including Phone
                })
                .ToListAsync();
        }

  

        public async Task<OrderDetailDto> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            // Fetch the order product including related entities
            var orderProduct = await _context.OrderProducts
                .Include(op => op.Customer)
                .Include(op => op.Address)
                .Include(op => op.Payments)
                .Include(op => op.OrderProductDetails)
                .ThenInclude(opd => opd.Product)
                .Include(op => op.OrderProductDetails) // Include order product details again for player
                .ThenInclude(opd => opd.Player)
                .FirstOrDefaultAsync(op => op.Id == orderId);

            if (orderProduct == null)
            {
                // Handle the case when no order is found (you might throw an exception or return null)
                return null;
            }

            // Map to OrderDetailDto
            var orderDetailDto = new OrderDetailDto
            {
                Id = orderProduct.Id,
                OrderCode = orderProduct.OrderCode,
                OrderDate = orderProduct.OrderDate,
                ShipmentFee = orderProduct.ShipmentFee,
                TotalPrice = orderProduct.TotalPrice,
                Note = orderProduct.Note,
                Status = orderProduct.Status,
                CustomerEmail = orderProduct.Customer?.Email,
                CustomerPhone = orderProduct.Customer?.Phone,
                CustomerFullName = orderProduct.Customer?.FullName,
                Address = new AddressDto
                {
                    AddressId = orderProduct.AddressId ?? 0, // Assuming AddressId is not null
                    CustomerId = orderProduct.Address?.CustomerId,
                    CityName = orderProduct.Address?.CityName,
                    City = orderProduct.Address?.City,
                    DistrictName = orderProduct.Address?.DistrictName,
                    District = orderProduct.Address?.District,
                    WardName = orderProduct.Address?.WardName,
                    Ward = orderProduct.Address?.Ward,
                    DetailedAddress = orderProduct.Address?.DetailedAddress,
                    Status = orderProduct.Address?.Status
                },
                Products = orderProduct.OrderProductDetails.Select(opd => new ProductDetailDto
                {
                    Id = opd.Id,
                    OrderProductId = opd.OrderProductId,
                    Product = new ProductorderDto
                    {
                        ProductId = opd.Product?.ProductId ?? 0, 
                        ProductName = opd.Product?.ProductName,
                        Avatar = opd.Product?.Avatar
                    },
                    Player = new PlayerProductorderDto
                    {
                        PlayerId = opd.Player?.PlayerId ?? 0, 
                        FullName = opd.Player?.FullName,
                        ShirtNumber = opd.Player?.ShirtNumber
                    },
                    CustomShirtNumber = opd.CustomShirtNumber,
                    CustomShirtName = opd.CustomShirtName,
                    Size = opd.Size,
                    Quantity = opd.Quantity,
                    Price = opd.Price,
                    Status = opd.Status
                }).ToList(),
                Payment = orderProduct.Payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderTicketId = p.OrderTicketId,
                    OrderProductId = p.OrderProductId,
                    TotalAmount = p.TotalAmount,
                    PaymentGateway = p.PaymentGateway,
                    Status = p.Status
                }).FirstOrDefault() 
            };

            return orderDetailDto;
        }
    }
}
