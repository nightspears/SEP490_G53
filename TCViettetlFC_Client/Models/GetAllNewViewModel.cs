namespace TCViettetlFC_Client.Models
{
    public class GetAllNewViewModel
    {
        public int Id { get; set; }

        public string? CreatorId { get; set; }

        public string? NewsCategory { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Image { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }
        public int? NewsCategoryId { get; set; }


    }
}
