using TCViettelFC_API.Models;

namespace TCViettelFC_API.Dtos.Season
{
    public class SeasonDto 
    {

        //public int SeasonId { get; set; }

        public string? SeasonName { get; set; }

        public DateTime? StartYear { get; set; }

        public DateTime? EndYear { get; set; }

        public int? Status { get; set; }
    }
    public class SeasonResponse
    {

        public int SeasonId { get; set; }

        public string? SeasonName { get; set; }

        public DateTime? StartYear { get; set; }

        public DateTime? EndYear { get; set; }

        public int? Status { get; set; }
    }
}
