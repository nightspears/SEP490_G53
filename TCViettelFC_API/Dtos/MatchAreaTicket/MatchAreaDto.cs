namespace TCViettelFC_API.Dtos.MatchAreaTicket
{
    public class MatchAreaDto
    {

        public int Id { get; set; }

        public string? OpponentName { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? StadiumName { get; set; }


    }
    public class MatchAreaRespone
    {

        public int AreaId { get; set; }
        public int MatchId { get; set; }
        public string? Stands { get; set; }
        public string? Floor { get; set; }
        public string? Section { get; set; }

        public int? AvailableSeats { get; set; }

        public string? OpponentName { get; set; }

        public string? StadiumName { get; set; }
        public decimal? Price { get; set; }
        public DateTime? MatchDate { get; set; }
    }

    public class MatchAreaRequest
    {

        public int AreaId { get; set; }

        public int MatchId { get; set; }

        public int SeatQuantity { get; set; }



    }
}
