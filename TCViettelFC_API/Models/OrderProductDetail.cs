namespace TCViettelFC_API.Models;

public partial class OrderProductDetail
{
    public int Id { get; set; }

    public int? OrderProductId { get; set; }

    public int? ProductId { get; set; }

    public int? PlayerId { get; set; }

    public string? CustomShirtNumber { get; set; }

    public string? CustomShirtName { get; set; }

    public string? Size { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual OrderProduct? OrderProduct { get; set; }

    public virtual Player? Player { get; set; }

    public virtual Product? Product { get; set; }
}
