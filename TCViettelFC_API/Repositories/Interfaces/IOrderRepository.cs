using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.Order;
using TCViettelFC_API.Models;
using static TCViettelFC_API.Repositories.Implementations.OrderRepository;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<TicketOrdersDto>> GetAllTicketOrders();
        Task<IEnumerable<OrderedSuppItemDto>> GetOrderedSuppItemByTicketOrderId(int id);
        Task<IEnumerable<OrderedTicketDto>> GetOrderedTicketByTicketOrderId(int id);

        Task<IEnumerable<OrderProductDto>> GetAllOrderProductsAsync();

        Task<OrderDetailDto> GetOrderDetailsByOrderIdAsync(int orderId);

        // New method to update the order status
        Task<bool> UpdateOrderStatusAsync(int orderId, int newStatus,int staffid);
        Task<bool> UpsertShipmentAsync(ShipmentDto shipmentDto);
        Task<IEnumerable<OrderProductCustomerDto>> GetOrdersByCustomerAccountIdAsync(int customerAccountId);
	}
}
