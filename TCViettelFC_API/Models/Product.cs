using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Avartar { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public string? Material { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CategoryId { get; set; }

    public int? Status { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; } = new List<OrderProductDetail>();

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();

    public virtual ICollection<ProductFile> ProductFiles { get; set; } = new List<ProductFile>();
}
