using System.ComponentModel.DataAnnotations;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Dtos
{
    public class GetNewDto
    {
        public int Id { get; set; }

        public string? CreatorId { get; set; }

        public string? NewsCategory { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Image { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }

        
    }
    public class CreateNewDto
    {
        public int? CreatorId { get; set; }
        public int? NewsCategoryId { get; set; }
        [MaxLength(255)]
        public string? Title { get; set; }
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }
    }

    public class UpdateNewDto
    {
        public int? CreatorId { get; set; }
        public int? NewsCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime? CreatedAt { get; set; } 
        
    }
}
