namespace TCViettelFC_API.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string? DiscountName { get; set; }

    public int? DiscountPercent { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidUntil { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
