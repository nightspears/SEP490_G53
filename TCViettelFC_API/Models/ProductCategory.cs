using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCViettelFC_API.Models;

public partial class ProductCategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [NotMapped]
    public string CreatedAtFormatted => CreatedAt?.ToString("dd/MM/yyyy");

}
