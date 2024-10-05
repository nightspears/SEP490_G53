using TCViettelFC_API.Models;
namespace TCViettelFC_API.Dtos.Matches
{
    public class MatchesAddDto 
    {

        public IFormFile? LogoUrl { get; set; }

        public bool? IsHome { get; set; }

        public string? OpponentName { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? StadiumName { get; set; }

        public int? Status { get; set; }


    }
}
