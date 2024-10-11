namespace TCViettetlFC_Client.Models
{
    public class TicketOrdersViewModel
    {
        public int id { get; set; }

        public DateTime? orderDate { get; set; }

        public decimal? totalAmount { get; set; }

        public string? customerEmail { get; set; }
    }
}
