namespace TCViettelFC_API.Dtos.Discount
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }

        public string? DiscountName { get; set; }

        public decimal? DiscountPercent { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidUntil { get; set; }

        public int? Status { get; set; }
    }
}
