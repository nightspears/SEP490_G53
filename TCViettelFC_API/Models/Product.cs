using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? PlayerId { get; set; }

    public int? SeasonId { get; set; }

    public int? CategoryId { get; set; }

    public string? ProductName { get; set; }

    public string? Avatar { get; set; }

    public decimal? Price { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public string? Material { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual ProductCategory? Category { get; set; }

    public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; } = new List<OrderProductDetail>();

    public virtual Player? Player { get; set; }

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();

    public virtual ICollection<ProductFile> ProductFiles { get; set; } = new List<ProductFile>();

    public virtual Season? Season { get; set; }
}
