namespace TCViettetlFC_Client.Models
{
    public class TicketOrderHistoryDto
    {
        public int id { get; set; }
        public DateTime? orderDate { get; set; }

        public decimal? totalAmount { get; set; }
    }
}
