using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.OrderTicket;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ITicketOrderRepository
    {
        Task<AddTicketResponseDto> AddOderedTicket(TicketOrderDto ticketOrdersDto, int? customerId = null);
        Task<List<int>> GetOrderedTicketsIdByOrderId(int orderId);
    }
}
