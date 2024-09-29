using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class SupplementaryItem
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<OrderTicketConcession> OrderTicketConcessions { get; set; } = new List<OrderTicketConcession>();
}
