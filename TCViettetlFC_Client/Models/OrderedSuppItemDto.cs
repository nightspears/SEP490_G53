namespace TCViettetlFC_Client.Models
{
    public class OrderedSuppItemDto
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public string? ItemName { get; set; }
        public string? ImageUrl { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
}
