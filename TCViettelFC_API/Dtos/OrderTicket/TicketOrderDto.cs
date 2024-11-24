namespace TCViettelFC_API.Dtos.OrderTicket
{
    public class TicketOrderDto
    {
        public AddCustomerDto? AddCustomerDto { get; set; }
        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? CustomerId { get; set; }
        // Thêm các danh sách OrderedTickets và OrderedSuppItems
        public List<OrderedTicketDto> OrderedTickets { get; set; } = new List<OrderedTicketDto>();

        public List<OrderedSuppItemDto> OrderedSuppItems { get; set; } = new List<OrderedSuppItemDto>();

        // Payment details
        public PaymentTicketDto? PaymentDto { get; set; }

    }
    public class OrderedTicketDto
    {
        public int? MatchId { get; set; }

        public int? AreaId { get; set; }

        public decimal? Price { get; set; }

        public int? Status { get; set; }

        public int? OrderId { get; set; }
    }
    public class OrderedSuppItemDto
    {
        public int? OrderId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
    public class AddCustomerDto
    {
       

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? FullName { get; set; }

        public int? Status { get; set; }
    }

    public class PaymentTicketDto
    {
        public int? OrderTicketId { get; set; }
        public int? OrderProductId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PaymentGateway { get; set; } // e.g.,"vnpay"
        public int? Status { get; set; } // e.g., 0 for "Pending", 1 for "Completed"
    }
}
