using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
    [NotMapped]
    public string FromFormatted => ValidFrom?.ToString("dd/MM/yyyy");
    [NotMapped]
    public string UntilFormatted => ValidUntil?.ToString("dd/MM/yyyy");
}
