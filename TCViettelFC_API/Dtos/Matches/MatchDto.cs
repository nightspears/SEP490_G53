using TCViettelFC_API.Models;
namespace TCViettelFC_API.Dtos.Matches

{
    public class MatchDto
    {
        public int Id { get; set; }

        public string? LogoUrl { get; set; }

        public bool? IsHome { get; set; }

        public string? OpponentName { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? StadiumName { get; set; }

        public int? Status { get; set; }


    }

    public class CheckMatch
    {
        public string NgayDa { get; set; } 
        public string TenDoiThu { get; set; } 
        public string TenSan { get; set; }
    }
}
