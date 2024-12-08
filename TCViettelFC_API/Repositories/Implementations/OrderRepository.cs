using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.Order;
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
                if (order.TotalAmount == null)
                {
                    throw new InvalidOperationException("TotalAmount cannot be null.");
                }
                string? customerEmail = null;
                if (order.Customer != null) // Check if Customer is null
                {
                    if (order.Customer.Email == null && order.Customer.AccountId != null)
                    {
                        var customeracc = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.CustomerId == order.Customer.AccountId);
                        customerEmail = customeracc?.Email; // Check if customeracc is null
                    }
                    else
                    {
                        customerEmail = order.Customer.Email;
                    }
                }

                var dto = new TicketOrdersDto()
                {
                    CustomerEmail = customerEmail,
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
                    Imageurl = order.Item.ImageUrl,
                    ItemName = order.Item.ItemName,
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
            var orders = await _context.OrderedTickets.Include(x => x.Area).Include(x => x.Match).Where(x => x.OrderId == id).ToListAsync();
            var results = new List<OrderedTicketDto>();
            foreach (var order in orders)
            {
                var dto = new OrderedTicketDto()
                {
                    Id = order.Id,
                    Vitri = "Khán đài " + order.Area.Stands + " - Tầng " + order.Area.Floor + " - Cửa " + order.Area.Section,
                    Trandau = order.Match.StadiumName + " - Thể Công Viettel vs " + order.Match.OpponentName + " - " + order.Match.MatchDate,
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
                    // Customer details (with null-checks)
                    Email = op.Customer != null ? op.Customer.Email : null,
                    Phone = op.Customer != null ? op.Customer.Phone : null,
                    FullName = op.Customer != null ? op.Customer.FullName : null,

                    // Staff details (with null-checks)
                    StaffId = op.Staff != null ? op.Staff.UserId : (int?)null,
                    StaffFullName = op.Staff != null ? op.Staff.FullName : null,
                    StaffEmail = op.Staff != null ? op.Staff.Email : null,
                    StaffPhone = op.Staff != null ? op.Staff.Phone : null,

                    // Shipment details (taking the first shipment if available)
                    ShipmentTrackingCode = op.Shipments.FirstOrDefault() != null ? op.Shipments.First().ShipmentTrackingCode : null,
                })
                .ToListAsync();
        }
        // Method to get orders by CustomerAccountId
        public async Task<IEnumerable<OrderProductCustomerDto>> GetOrdersByCustomerAccountIdAsync(int customerAccountId)
        {
            var orders = await _context.OrderProducts
                .Where(op => op.Customer.AccountId == customerAccountId)
                .Include(op => op.Customer) // Optional: Include Customer if you need customer details
                .Include(op => op.OrderProductDetails) // Optional: Include OrderProductDetails if you need them
                .Include(op => op.Payments) // Optional: Include Payments if you need them
                .Include(op => op.Shipments) // Optional: Include Shipments if you need them
                .Select(op => new OrderProductCustomerDto
                {
                    Id = op.Id,
                    OrderCode = op.OrderCode,
                    OrderDate = op.OrderDate,
                    ShipmentFee = op.ShipmentFee,
                    TotalPrice = op.TotalPrice,
                    AddressId = op.AddressId,
                    Status = op.Status,
                    // Add other necessary fields here, such as Customer info or Payment details
                })
                .ToListAsync();

            return orders;
        }
        public class OrderProductCustomerDto
        {
            public int Id { get; set; }
            public string? OrderCode { get; set; }
            public DateTime? OrderDate { get; set; }
            public decimal? ShipmentFee { get; set; }
            public decimal? TotalPrice { get; set; }
            public int? AddressId { get; set; }
            public int? Status { get; set; }
            // Add other properties as needed
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

        // Method to update the status of an order by orderId
        public async Task<bool> UpdateOrderStatusAsync(int orderId, int newStatus, int staffid)
        {
            var order = await _context.OrderProducts.FindAsync(orderId);
            if (order == null)
            {
                return false; // Order not found
            }

            order.Status = newStatus;
            order.StaffId = staffid;
            _context.OrderProducts.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpsertShipmentAsync(ShipmentDto shipmentDto)
        {
            var existingShipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.OrderId == shipmentDto.OrderId);

            if (existingShipment != null)
            {
                // Update existing shipment details
                existingShipment.ShipmentTrackingCode = shipmentDto.ShipmentTrackingCode;
                existingShipment.ShipmentDate = shipmentDto.ShipmentDate;
                existingShipment.DeliveryDate = shipmentDto.DeliveryDate;
                existingShipment.Status = shipmentDto.Status;
                _context.Shipments.Update(existingShipment);
            }
            else
            {
                // Map DTO to Shipment entity for new entry
                var newShipment = new Shipment
                {
                    OrderId = shipmentDto.OrderId,
                    ShipmentTrackingCode = shipmentDto.ShipmentTrackingCode,
                    ShipmentDate = shipmentDto.ShipmentDate,
                    DeliveryDate = shipmentDto.DeliveryDate,
                    Status = shipmentDto.Status
                };
                await _context.Shipments.AddAsync(newShipment);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
