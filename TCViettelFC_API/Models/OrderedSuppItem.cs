using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderedSuppItem
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ItemId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual SupplementaryItem? Item { get; set; }

    public virtual TicketOrder? Order { get; set; }
}
