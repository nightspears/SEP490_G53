using TCViettelFC_API.Dtos.Order;
using TCViettelFC_API.Dtos.OrderTicket;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ITicketOrderRepository
    {
        Task AddOderedTicket(TicketOrderDto ticketOrdersDto, int? customerId = null);
    }
}
