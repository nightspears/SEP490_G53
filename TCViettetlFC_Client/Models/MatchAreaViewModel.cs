namespace TCViettetlFC_Client.Models
{
   
    public class MatchArea {

        public int AreaId { get; set; }
        public int MatchId { get; set; }
        public string? Stands { get; set; }
        public string? Floor { get; set; }
        public string? Section { get; set; }

        public int? AvailableSeats { get; set; }

        public string? OpponentName { get; set; }

        public string? StadiumName { get; set; }
        public decimal? Price { get; set; }

    }


}
