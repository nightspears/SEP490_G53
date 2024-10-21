using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string? FullName { get; set; }

    public int? ShirtNumber { get; set; }

    public int? SeasonId { get; set; }

    public string? Position { get; set; }

    public DateTime? JoinDate { get; set; }

    public DateTime? OutDate { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Season? Season { get; set; }
}
