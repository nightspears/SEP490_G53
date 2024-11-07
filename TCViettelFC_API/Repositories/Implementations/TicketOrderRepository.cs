using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.CheckOut;
using TCViettelFC_API.Dtos.OrderTicket;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class TicketOrderRepository : ITicketOrderRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public TicketOrderRepository(Sep490G53Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<AddTicketResponseDto> AddOderedTicket(TicketOrderDto ticketOrdersDto, int? customerId = null)
        {
            var obj = new AddTicketResponseDto();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create ticket order using the customerId (whether newly created or passed in)
                    var ticketOrder = new TicketOrder
                    {
                        OrderDate = ticketOrdersDto.OrderDate,
                        TotalAmount = ticketOrdersDto.TotalAmount
                    };
                    if (customerId == null)
                    {
                        if (ticketOrdersDto.AddCustomerDto == null ||
                            string.IsNullOrEmpty(ticketOrdersDto.AddCustomerDto.Email) ||
                            string.IsNullOrEmpty(ticketOrdersDto.AddCustomerDto.Phone))
                        {
                            throw new Exception("Thông tin khách hàng yêu cầu nhưng không được cung cấp.");
                        }

                        // Create a new customer if customerId is not provided
                        var newCustomer = new Customer
                        {
                            Email = ticketOrdersDto.AddCustomerDto.Email,
                            Phone = ticketOrdersDto.AddCustomerDto.Phone,
                            FullName = ticketOrdersDto.AddCustomerDto.FullName,
                            Status = ticketOrdersDto.AddCustomerDto.Status ?? 0
                        };

                        _context.Customers.Add(newCustomer);
                        await _context.SaveChangesAsync();
                        customerId = newCustomer.CustomerId;
                        obj.CustomerEmail = newCustomer.Email;
                        ticketOrder.Customer = newCustomer;
                    }
                    else
                    {
                        var customer = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.CustomerId == customerId);
                        obj.CustomerEmail = customer.Email;
                        var cus = new Customer()
                        {
                            AccountId = customerId.Value
                        };
                        ticketOrder.Customer = cus;
                    }





                    _context.TicketOrders.Add(ticketOrder);
                    await _context.SaveChangesAsync();
                    var orderId = ticketOrder.Id;

                    // Add ordered tickets if any
                    if (ticketOrdersDto.OrderedTickets != null)
                    {
                        foreach (var orderedTicketDto in ticketOrdersDto.OrderedTickets)
                        {
                            if (orderedTicketDto.MatchId == 0 || orderedTicketDto.AreaId == 0)
                            {
                                throw new Exception("Thông tin vé đặt hàng không đầy đủ.");
                            }
                            ticketOrder.OrderedTickets.Add(new OrderedTicket
                            {
                                OrderId = orderId,
                                MatchId = orderedTicketDto.MatchId,
                                AreaId = orderedTicketDto.AreaId,
                                Price = orderedTicketDto.Price,
                                Status = orderedTicketDto.Status,
                            });
                        }
                    }

                    // Add ordered supplementary items if any
                    if (ticketOrdersDto.OrderedSuppItems != null)
                    {
                        foreach (var orderedSuppItemDto in ticketOrdersDto.OrderedSuppItems)
                        {
                            ticketOrder.OrderedSuppItems.Add(new OrderedSuppItem
                            {
                                OrderId = orderId,
                                ItemId = orderedSuppItemDto.ItemId,
                                Quantity = orderedSuppItemDto.Quantity,
                                Price = orderedSuppItemDto.Price
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    if (ticketOrdersDto.PaymentDto != null)
                    {
                        var payment = new Payment
                        {
                            OrderTicketId = orderId,
                            TotalAmount = ticketOrdersDto.PaymentDto.TotalAmount,
                            PaymentGateway = ticketOrdersDto.PaymentDto.PaymentGateway,
                            Status = ticketOrdersDto.PaymentDto.Status ?? 0 // Default to pending if not provided
                        };

                        _context.Payments.Add(payment);
                        await _context.SaveChangesAsync();
                    }

                    // Update seats for ordered tickets
                    foreach (var orderedTicket in ticketOrder.OrderedTickets)
                    {
                        var matchAreaTicket = await _context.MatchAreaTickets
                            .FirstOrDefaultAsync(mat => mat.MatchId == orderedTicket.MatchId && mat.AreaId == orderedTicket.AreaId);

                        if (matchAreaTicket != null && matchAreaTicket.AvailableSeats > 0)
                        {
                            matchAreaTicket.AvailableSeats -= 1;
                            if (matchAreaTicket.AvailableSeats < 0)
                            {
                                throw new Exception("Không còn chỗ trống cho khu vực đã chọn.");
                            }
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy chỗ trống hoặc khu vực không tồn tại.");
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    obj.OrderId = orderId;
                    return obj;


                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Lỗi trong TicketOrderRepository.AddOrderedTicket: {ex.Message}");
                    throw new Exception("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex);
                }
            }
        }

        public async Task<List<int>> GetOrderedTicketsIdByOrderId(int orderId)
        {
            return await _context.OrderedTickets.Where(x => x.OrderId == orderId).Select(x => x.Id).ToListAsync();
        }
    }

}




