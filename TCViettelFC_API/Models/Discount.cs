using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string? DiscountName { get; set; }

    public decimal? DiscountPercent { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidUntil { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
}
