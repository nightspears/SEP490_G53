namespace TCViettetlFC_Client.Models
{
    public class TicketOrderDetailModel
    {
        public List<OrderedSuppItemDto> Sup { get; set; }
        public List<OrderedTicketDto> Ticket { get; set; }
    }
}
