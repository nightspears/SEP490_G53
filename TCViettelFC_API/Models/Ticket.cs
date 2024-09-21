using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int? MatchId { get; set; }

    public int? OrderTicketId { get; set; }

    public string? SeatNumber { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual Match? Match { get; set; }

    public virtual OrderTicket? OrderTicket { get; set; }
}
