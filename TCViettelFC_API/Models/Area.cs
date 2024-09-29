using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Area
{
    public int Id { get; set; }

    public string? Stands { get; set; }

    public string? Section { get; set; }

    public string? Rows { get; set; }

    public decimal? Price { get; set; }

    public int? NumberOfTicket { get; set; }

    public int? NumberOfChair { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
