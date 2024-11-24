using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCViettelFC_API.Models;

public partial class Season
{
    public int SeasonId { get; set; }

    public string? SeasonName { get; set; }

    public DateTime? StartYear { get; set; }

    public DateTime? EndYear { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [NotMapped]
    public string StartFormatted => StartYear?.ToString("dd/MM/yyyy");
    [NotMapped]
    public string EndFormatted => EndYear?.ToString("dd/MM/yyyy");
}
