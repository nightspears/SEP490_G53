using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string? FullName { get; set; }

    public int? ShirtNumber { get; set; }

    public string? Position { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
