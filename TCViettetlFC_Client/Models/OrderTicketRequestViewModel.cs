namespace TCViettetlFC_Client.Models
{
    public class OrderTicketRequestViewModel
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public List<OrderedTicket> OrderedTickets { get; set; }
        public List<OrderedSuppItem> OrderedSuppItems { get; set; }
        public AddCustomerDto AddCustomerDto { get; set; }
    }
    public class OrderedTicket
    {
        public int MatchId { get; set; }
        public int AreaId { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public int OrderId { get; set; }
    }

    public class OrderedSuppItem
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class AddCustomerDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public int Status { get; set; }
    }
}
