using TCViettelFC_API.Dtos.TiketOrder;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class TiketOrderRepository: ITiketOrderRepository
    {
        private readonly Sep490G53Context _context;
        public TiketOrderRepository(Sep490G53Context context)
        {
            _context = context;
        }
        public TicketOrder CreateOrUpdateOrder(int customerId)
        {
            var order = _context.TicketOrders.FirstOrDefault(o => o.CustomerId == customerId );
            if (order == null)
            {
                order = new TicketOrder { CustomerId = customerId, OrderDate = DateTime.Now, TotalAmount = 0 };
                _context.TicketOrders.Add(order);
            }
            return order;
        }

        public bool AddTicketToOrder(int orderId, TicketDetailDto ticketDetail)
        {
            var ticket = new OrderedTicket
            {
                MatchId = ticketDetail.MatchId,
                AreaId = ticketDetail.AreaId,
                Price = _context.Areas.FirstOrDefault(a => a.Id == ticketDetail.AreaId)?.Price ?? 0,
                Status = 0,
                OrderId = orderId
            };
            _context.OrderedTickets.Add(ticket);
            return true;
        }

        public bool CheckTicketAvailabilityAndUpdate(int matchId, int areaId, int quantity)
        {
            var matchAreaTicket = _context.MatchAreaTickets.FirstOrDefault(mat => mat.MatchId == matchId && mat.AreaId == areaId);
            if (matchAreaTicket != null && matchAreaTicket.AvailableSeats >= quantity)
            {
                matchAreaTicket.AvailableSeats -= quantity;
                return true;
            }
            return false;
        }
        public void AddSupplementaryItemToOrder(int orderId, SupplementaryItemDto itemDetail)
        {
            var orderedItem = new OrderedSuppItem
            {
                OrderId = orderId,
                ItemId = itemDetail.ItemId,
                Quantity = itemDetail.Quantity,
                Price = itemDetail.Price
            };
            _context.OrderedSuppItems.Add(orderedItem);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
