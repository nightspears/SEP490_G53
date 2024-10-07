namespace TCViettetlFC_Client.Models
{
    public class CategoryNewViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatorName { get; set; } 
        public int Status { get; set; }
    }
}
