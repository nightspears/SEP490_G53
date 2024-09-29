using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderTicketConcession
{
    public int OrderTicketId { get; set; }

    public int ConcessionId { get; set; }

    public int? Quantity { get; set; }

    public virtual SupplementaryItem Concession { get; set; } = null!;

    public virtual OrderTicket OrderTicket { get; set; } = null!;
}
