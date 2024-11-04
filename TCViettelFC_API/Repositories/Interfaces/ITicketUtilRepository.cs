using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ITicketUtilRepository
    {
        Task<int> SendTicketViaEmailAsync(List<int> ticketIds, string email);
        Task<int> VerifyTicketAsync(OrderedTicket ticket);
        Task<OrderedTicket> GetOrderedTicketByIdAsync(int id);

    }
}
