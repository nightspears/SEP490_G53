using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class OrderProduct
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? OrderCode { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? AddressId { get; set; }

    public int? Status { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; } = new List<OrderProductDetail>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public virtual User? User { get; set; }
}
