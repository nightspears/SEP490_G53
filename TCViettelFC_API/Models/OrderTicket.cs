using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderTicket
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? UserId { get; set; }

    public int? TotalTicket { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual CustomerTicket? Customer { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User? User { get; set; }
}
