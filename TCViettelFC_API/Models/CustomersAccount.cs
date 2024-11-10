using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class CustomersAccount
{
    public int CustomerId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<PersonalAddress> PersonalAddresses { get; set; } = new List<PersonalAddress>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
