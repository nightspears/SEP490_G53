namespace TCViettelFC_API.Dtos.TiketOrder
{
    public class TiketOrderDto
    {
    }
    public class TicketOrderCreateDto
    {
        public int CustomerId { get; set; }
        public List<TicketDetailDto> Tickets { get; set; }
        public List<SupplementaryItemDto> SupplementaryItems { get; set; }
    }

    public class TicketDetailDto
    {
        public int MatchId { get; set; }
        public int AreaId { get; set; }
        public int Quantity { get; set; }
    }
    public class SupplementaryItemDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }  // Giá có thể được đặt trước hoặc tính toán trong repository
    }

    
}
