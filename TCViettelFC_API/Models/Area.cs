using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Area
{
    public int Id { get; set; }

    public string? Stands { get; set; }

    public string? Floor { get; set; }

    public string? Section { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<MatchAreaTicket> MatchAreaTickets { get; set; } = new List<MatchAreaTicket>();

    public virtual ICollection<OrderedTicket> OrderedTickets { get; set; } = new List<OrderedTicket>();
}
