namespace TCViettelFC_API.Dtos
{
    public class TicketOrderHistoryDto
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}
