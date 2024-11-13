namespace TCViettelFC_API.Dtos
{
    public class AreaDto
    {
        public int Id { get; set; }

        public string? Stands { get; set; }

        public string? Floor { get; set; }

        public string? Section { get; set; }

        public decimal? Price { get; set; }

        public int? Status { get; set; }
    }

    public class MatchAreaTicketDto
    {
        public int MatchId { get; set; }
        public int AreaId { get; set; }
        public int? AvailableSeats { get; set; }
    }
}
