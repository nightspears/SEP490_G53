using TCViettelFC_API.Dtos.TiketOrder;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ITiketOrderRepository
    {
        
        TicketOrder CreateOrUpdateOrder(int customerId);
        bool AddTicketToOrder(int orderId, TicketDetailDto ticketDetail);
        bool CheckTicketAvailabilityAndUpdate(int matchId, int areaId, int quantity);
        void AddSupplementaryItemToOrder(int orderId, SupplementaryItemDto itemDetail);
        void SaveChanges();
    }
}
