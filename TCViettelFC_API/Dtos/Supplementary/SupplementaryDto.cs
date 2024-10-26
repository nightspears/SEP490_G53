namespace TCViettelFC_API.Dtos.Supplementary
{
   
    public class SupplementaryDto
    {
        public int? ItemId { get; set; } // Optional for create, required for update
        public string? ItemName { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
    }
}
