using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderTicketId { get; set; }

    public int? OrderProductId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentGateway { get; set; }

    public int? Status { get; set; }

    public virtual OrderProduct? OrderProduct { get; set; }

    public virtual TicketOrder? OrderTicket { get; set; }
}
