namespace TCViettelFC_API.Dtos.Supplementary
{

    public class SupplementaryDto
    {
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
        public IFormFile? Imageurl { get; set; }
    }


    public class SupplementaryRespone
    {
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
        public string Image { get; set; }
    }
}
