namespace TCViettetlFC_Client.Models
{
    public class UpdateNewViewModel
    {
        public int? CreatorId { get; set; }
        public int NewsCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
