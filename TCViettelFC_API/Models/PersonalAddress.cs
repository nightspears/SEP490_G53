using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class PersonalAddress
{
    public int AddressId { get; set; }

    public int? CustomerId { get; set; }

    public string? CityName { get; set; }

    public string? City { get; set; }

    public string? DistrictName { get; set; }

    public string? District { get; set; }

    public string? WardName { get; set; }

    public string? Ward { get; set; }

    public string? DetailedAddress { get; set; }

    public int? Status { get; set; }

    public virtual CustomersAccount? Customer { get; set; }
}
