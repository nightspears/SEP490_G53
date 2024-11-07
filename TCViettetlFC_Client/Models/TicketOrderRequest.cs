namespace TCViettetlFC_Client.Models
{
    public class TicketOrderRequest
    {
        public AddCustomerDto addCustomerDto { get; set; }
        public DateTime orderDate { get; set; }
        public decimal totalAmount { get; set; }
        public int? customerId { get; set; }
        public List<OrderedTicket> orderedTickets { get; set; }
        public List<OrderedSupplementaryItem> orderedSuppItems { get; set; }
        // Payment details
        public PaymentDto? paymentDto { get; set; }
    }

    public class OrderedSupplementaryItem
    {
        public int orderId { get; set; } // OrderId can be 0 for new items
        public int itemId { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
    public class OrderedTicket
    {
        public int matchId { get; set; }
        public int areaId { get; set; }
        public decimal price { get; set; }
        public int status { get; set; }
        public int orderId { get; set; }
    }
    public class AddCustomerDto
    {
        public string email { get; set; }
        public string phone { get; set; }
        public string fullName { get; set; }
        public int status { get; set; }
    }
    public class PaymentDto
    {
        public int? orderTicketId { get; set; }
        public int? orderProductId { get; set; }
        public decimal? totalAmount { get; set; }
        public string? paymentGateway { get; set; } // e.g., "Credit Card", "PayPal"
        public int? status { get; set; } // e.g., 0 for "Pending", 1 for "Completed"
    }
}
