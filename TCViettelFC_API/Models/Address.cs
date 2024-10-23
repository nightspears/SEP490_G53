using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int? CustomerId { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public string? DetailedAddress { get; set; }

    public int? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
