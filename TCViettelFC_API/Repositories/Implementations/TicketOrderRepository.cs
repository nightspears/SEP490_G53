using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;
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
                    // Nếu cả CustomerId và AddCustomerDto đều null
                    if (customerId == null && ticketOrdersDto.AddCustomerDto == null)
                    {
                        throw new Exception("Thông tin khách hàng yêu cầu nhưng không được cung cấp.");
                    }
                    var ticketOrder = new TicketOrder
                    {
                        OrderDate = ticketOrdersDto.OrderDate,
                        TotalAmount = ticketOrdersDto.TotalAmount,


                    };
                    if (customerId == null)
                    {
                       
                        if (ticketOrdersDto.AddCustomerDto == null ||
                            string.IsNullOrEmpty(ticketOrdersDto.AddCustomerDto.Email) ||
                            string.IsNullOrEmpty(ticketOrdersDto.AddCustomerDto.Phone) ||
                            string.IsNullOrEmpty(ticketOrdersDto.AddCustomerDto.FullName))
                        {
                            throw new Exception("Thông tin khách hàng yêu cầu nhưng không được cung cấp.");
                        }
                        // Kiểm tra tính hợp lệ của thông tin khách hàng
                        ValidateCustomerInformation(ticketOrdersDto.AddCustomerDto);

                        var newCustomer = new Customer
                        {
                            Email = ticketOrdersDto.AddCustomerDto.Email,
                            Phone = ticketOrdersDto.AddCustomerDto.Phone,
                            FullName = ticketOrdersDto.AddCustomerDto.FullName,
                            Status = ticketOrdersDto.AddCustomerDto.Status ?? 0
                        };



                        obj.CustomerEmail = newCustomer.Email;
                        ticketOrder.Customer = newCustomer;


                    }
                    else
                    {
                        var customer = await _context.CustomersAccounts.FirstOrDefaultAsync(x => x.CustomerId == customerId);
                        obj.CustomerEmail = customer.Email;
                        var cus = new Customer()
                        {
                            AccountId = customerId.Value,
                            FullName= customer.FullName,
                            Email= customer.Email
                           
                        };
                        ticketOrder.Customer = cus;
                    }

                    // Create ticket order using the customerId (whether newly created or passed in)



                    _context.TicketOrders.Add(ticketOrder);
                    await _context.SaveChangesAsync();
                    var orderId = ticketOrder.Id;

                    if (ticketOrdersDto.OrderedTickets.Count <= 0)
                    {
                        throw new Exception("Không có vé được chọn để mua");
                    }
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
                                Price = orderedSuppItemDto.Price,
                                Status = 0

                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    if(ticketOrdersDto.PaymentDto == null)
                    {
                        throw new Exception("Payment là thông tin bắt buộc");
                    }
                    if (ticketOrdersDto.PaymentDto != null)
                    {
                        if (ticketOrdersDto.PaymentDto.OrderTicketId == null)
                        {
                            throw new Exception("OrderTicketId không được phép null.");
                        }
                        if (ticketOrdersDto.PaymentDto.TotalAmount == null)
                        {
                            throw new Exception("TotalAmount không được phép null.");
                        }
                        // Kiểm tra sự khớp giữa tổng tiền của đơn hàng và tổng tiền thanh toán
                        if (ticketOrdersDto.TotalAmount != ticketOrdersDto.PaymentDto.TotalAmount)
                        {
                            throw new Exception("Tổng tiền thanh toán không khớp với tổng tiền của đơn hàng.");
                        }
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
                            matchAreaTicket.Count -= 1;

                            
                        }
                        else if (matchAreaTicket.AvailableSeats < 0)
                        {
                            throw new Exception("Không còn chỗ trống cho khu vực đã chọn.");

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

        public async Task<int> CheckCustomerlimitBuyTicket(int matchId, int? customerId = null)
        {
            if (!customerId.HasValue)
            {

                return -1;
            }

            var totalTickets = await (from t in _context.TicketOrders
                                      join c in _context.Customers on t.CustomerId equals c.CustomerId
                                      join ot in _context.OrderedTickets on t.Id equals ot.OrderId
                                      where c.AccountId == customerId && ot.MatchId == matchId
                                      select ot.Id)
                           .CountAsync();

            return totalTickets;
        }

        public async Task<int> CheckCustomerNoAccountBuyTicket(int matchId, string? email,int? customerId = null)
        {
            if (customerId.HasValue)
            {
                return -1;
            }
            var totalTicket = await(from t in _context.TicketOrders
                                    join c in _context.Customers on t.CustomerId equals c.CustomerId
                                    join ot in _context.OrderedTickets on t.Id equals ot.OrderId
                                    where c.Email == email && ot.MatchId == matchId
                                    select ot.Id) .CountAsync();
            return totalTicket;
        }

        public async Task<List<int>> GetOrderedTicketsIdByOrderId(int orderId)
        {
            return await _context.OrderedTickets.Where(x => x.OrderId == orderId).Select(x => x.Id).ToListAsync();
        }
        private void ValidateCustomerInformation(AddCustomerDto addCustomerDto)
        {
            // Kiểm tra tính hợp lệ của email
            var email = addCustomerDto.Email;
            if (email.Contains(" "))
            {
                throw new Exception("Email không hợp lệ: không được chứa dấu cách.");
            }

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                throw new Exception("Email không hợp lệ.");
            }

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                throw new Exception("Email không hợp lệ: Định dạng email không đúng.");
            }

            // Kiểm tra tính hợp lệ của số điện thoại
            var phone = addCustomerDto.Phone;
            if (string.IsNullOrWhiteSpace(phone) || phone.Contains(" "))
            {
                throw new Exception("Số điện thoại không hợp lệ: không được chứa dấu cách.");
            }

            if (!Regex.IsMatch(phone, @"^[\d\+]+$"))
            {
                throw new Exception("Số điện thoại không hợp lệ: chỉ được phép chứa các chữ số và ký tự '+'.");
            }

            // Kiểm tra tính hợp lệ của tên đầy đủ
            var fullName = addCustomerDto.FullName;
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length == 0)
            {
                throw new Exception("Tên đầy đủ không hợp lệ: không được chỉ chứa dấu cách.");
            }

            if (fullName != fullName.Trim())
            {
                throw new Exception("Tên đầy đủ không hợp lệ: không được chứa dấu cách ở đầu và cuối.");
            }

        }

    }

}




