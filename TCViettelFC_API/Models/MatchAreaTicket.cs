using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class MatchAreaTicket
{
    public int MatchId { get; set; }

    public int AreaId { get; set; }

    public int? AvailableSeats { get; set; }

    public int? Count { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual Match Match { get; set; } = null!;
}
