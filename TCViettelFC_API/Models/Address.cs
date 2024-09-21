using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Address
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? Description { get; set; }

    public string? DetailedAddress { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual User? User { get; set; }
}
