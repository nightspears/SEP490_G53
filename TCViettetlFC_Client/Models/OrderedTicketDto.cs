namespace TCViettetlFC_Client.Models
{
    public class OrderedTicketDto
    {
        public int Id { get; set; }

        public int? MatchId { get; set; }

        public int? AreaId { get; set; }

        public decimal? Price { get; set; }

        public int? Status { get; set; }

        public int? OrderId { get; set; }
    }
}
