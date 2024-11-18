namespace TCViettetlFC_Client.Models
{
    public class UpdateNewViewModel
    {
        public int? creatorId { get; set; }
        public int newsCategoryId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public IFormFile? image { get; set; }
        public DateTime? createdAt { get; set; }
       
        
    }
}
