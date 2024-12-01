namespace TCViettetlFC_Client.Models
{
    public class CustomerNewsModel
    {
        public int Id { get; set; }

        public int? CreatorId { get; set; }

        public string? NewsCategory { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Image { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }

    }
}
