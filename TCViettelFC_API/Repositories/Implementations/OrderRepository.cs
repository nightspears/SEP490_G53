using Microsoft.EntityFrameworkCore;
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
    }
}
