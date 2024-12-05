using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.OrderTicket;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ITicketOrderRepository
    {
        Task<AddTicketResponseDto> AddOderedTicket(TicketOrderDto ticketOrdersDto, int? customerId = null);
        Task<int> CheckCustomerlimitBuyTicket(int matchId,int? customerId = null);
        Task<int> CheckCustomerNoAccountBuyTicket(int matchId, string? email,int? customerId = null);
    }
}
