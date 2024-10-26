namespace TCViettelFC_API.Dtos.BuyTiketController
{
    public class BuyTiketDto
    {
    }
    public class MatchAreaTiketDto
    {
        public int MatchId { get; set; }

        public int AreaId { get; set; }

        public int? AvailableSeats { get; set; }
    }
    public class TiketOrderDto
    {
        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? CustomerId { get; set; }
    }
}
