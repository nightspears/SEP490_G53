using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class TicketOrder
{
    public int Id { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderedSuppItem> OrderedSuppItems { get; set; } = new List<OrderedSuppItem>();

    public virtual ICollection<OrderedTicket> OrderedTickets { get; set; } = new List<OrderedTicket>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
