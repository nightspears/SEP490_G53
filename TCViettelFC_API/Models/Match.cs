using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Match
{
    public int Id { get; set; }

    public string? LogoUrl { get; set; }

    public bool? IsHome { get; set; }

    public string? OpponentName { get; set; }

    public DateTime? MatchDate { get; set; }

    public string? StadiumName { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
