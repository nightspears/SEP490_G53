using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Profile
{
    public int ProfileId { get; set; }

    public int? CustomerId { get; set; }

    public string? FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
