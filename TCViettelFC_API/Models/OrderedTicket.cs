using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderedTicket
{
    public int Id { get; set; }

    public int? MatchId { get; set; }

    public int? AreaId { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public int? OrderId { get; set; }

    public virtual Area? Area { get; set; }

    public virtual Match? Match { get; set; }

    public virtual TicketOrder? Order { get; set; }
}
