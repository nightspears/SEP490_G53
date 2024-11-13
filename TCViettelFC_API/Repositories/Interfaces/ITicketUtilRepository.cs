using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ITicketUtilRepository
    {
        Task<int> SendTicketViaEmailAsync(int orderId, string email);
        Task<int> VerifyTicketAsync(OrderedTicket ticket);
        Task<List<OrderedTicket>> GetOrderedTicketsByOrderId(int orderId);
        Task<List<OrderedSuppItem>> GetOrderedSuppByOrderId(int orderId);
        Task<OrderedTicket> GetOrderedTicketByIdAsync(int orderId);
        Task<List<VerifySupDto>> VerifyItemAsync(List<OrderedSuppItem> item);


    }
}
