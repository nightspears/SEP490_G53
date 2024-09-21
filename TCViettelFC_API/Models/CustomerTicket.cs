using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class CustomerTicket
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();
}
