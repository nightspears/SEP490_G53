namespace TCViettelFC_API.Dtos
{
    public class GetNewCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatorName { get; set; } // Assuming you want to include creator details
        public int Status { get; set; }
    }
}
