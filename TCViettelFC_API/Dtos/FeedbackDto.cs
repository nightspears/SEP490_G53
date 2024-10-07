using TCViettelFC_API.Models;

namespace TCViettelFC_API.Dtos
{
    public class FeedbackDto
    {
        public int Id { get; set; }

        public int? CreatorId { get; set; }

        public int? ResponderId { get; set; }

        public string? Content { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }

        public int UserId { get; set; }

        public int CustomerId { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

    }
    public class UpdateFeedbackDto
    {
        public int? ResponderId { get; set; }
        public int? Status { get; set; }
    }
}
