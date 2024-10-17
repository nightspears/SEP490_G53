namespace TCViettelFC_API.Dtos.Order
{
    public class TicketOrdersDto
    {
        public int Id { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? CustomerEmail { get; set; }
    }
}
