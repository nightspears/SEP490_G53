namespace TCViettelFC_API.Dtos.Order
{
    public class OrderedTicketDto
    {
        public int Id { get; set; }

        public int? MatchId { get; set; }
        public string? Vitri { get; set; }
        public string? Trandau { get; set; }

        public int? AreaId { get; set; }

        public decimal? Price { get; set; }

        public int? Status { get; set; }

        public int? OrderId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
