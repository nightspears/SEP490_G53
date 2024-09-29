using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Season
{
    public int SeasonId { get; set; }

    public string? SeasonName { get; set; }

    public DateTime? StartYear { get; set; }

    public DateTime? EndYear { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
