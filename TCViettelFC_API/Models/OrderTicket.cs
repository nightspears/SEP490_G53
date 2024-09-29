using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderTicket
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? TicketId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderTicketConcession> OrderTicketConcessions { get; set; } = new List<OrderTicketConcession>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Ticket? Ticket { get; set; }
}
