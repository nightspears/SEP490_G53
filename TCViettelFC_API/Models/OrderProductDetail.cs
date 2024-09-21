using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderProductDetail
{
    public int Id { get; set; }

    public int? OrderProductId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual OrderProduct? OrderProduct { get; set; }

    public virtual Product? Product { get; set; }
}
