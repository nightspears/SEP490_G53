namespace TCViettelFC_API.Models;

public partial class SupplementaryItem
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<OrderedSuppItem> OrderedSuppItems { get; set; } = new List<OrderedSuppItem>();
}
