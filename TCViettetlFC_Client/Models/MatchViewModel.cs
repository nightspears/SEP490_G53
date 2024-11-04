namespace TCViettetlFC_Client.Models
{
    public class MatchViewModel
    {
        public int Id { get; set; }

        public string? LogoUrl { get; set; }

        public bool IsHome { get; set; }

        public string? OpponentName { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? StadiumName { get; set; }

        public int? Status { get; set; }
    }
}
