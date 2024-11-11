namespace TCViettelFC_API.Dtos
{
    public class ShipmentDto
    {
        public int? OrderId { get; set; }
        public string? ShipmentTrackingCode { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? Status { get; set; }
    }
}
