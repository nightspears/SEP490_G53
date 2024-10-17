using TCViettelFC_API.Dtos.Order;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<TicketOrdersDto>> GetAllTicketOrders();
        Task<IEnumerable<OrderedSuppItemDto>> GetOrderedSuppItemByTicketOrderId(int id);
        Task<IEnumerable<OrderedTicketDto>> GetOrderedTicketByTicketOrderId(int id);
    }
}
