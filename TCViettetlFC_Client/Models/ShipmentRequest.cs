using Newtonsoft.Json;

namespace TCViettelFC_Client.Models
{
    public class ShipmentRequestWrapper
    {
        [JsonProperty("shipment")]
        public ShipmentRequest Shipment { get; set; }
    }

    public class ShipmentRequest
    {
        [JsonProperty("rate")]
        public string Rate { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("address_from")]
        public AddressFrom AddressFrom { get; set; }

        [JsonProperty("address_to")]
        public AddressTo AddressTo { get; set; }

        [JsonProperty("parcel")]
        public Parcel Parcel { get; set; }
    }

    public class AddressFrom
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("ward")]
        public string Ward { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public class AddressTo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("ward")]
        public string Ward { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public class Parcel
    {
        [JsonProperty("cod")]
        public int Cod { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("metadata")]
        public string Metadata { get; set; }
    }

    public class ShipmentResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("shipment_status")]
        public int ShipmentStatus { get; set; }

        [JsonProperty("shipment_status_txt")]
        public string ShipmentStatusTxt { get; set; }

        [JsonProperty("cod")]
        public int Cod { get; set; }

        [JsonProperty("fee")]
        public int Fee { get; set; }

        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; }

        [JsonProperty("carrier")]
        public string Carrier { get; set; }

        [JsonProperty("carrier_short_name")]
        public string CarrierShortName { get; set; }

        [JsonProperty("sorting_code")]
        public string SortingCode { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }
}
