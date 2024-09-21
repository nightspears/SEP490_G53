using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Shipment
{
    public int ShipmentId { get; set; }

    public int? OrderProductId { get; set; }

    public DateTime? ShipmentDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public int? Status { get; set; }

    public virtual OrderProduct? OrderProduct { get; set; }
}
