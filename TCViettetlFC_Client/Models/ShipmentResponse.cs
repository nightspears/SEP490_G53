namespace TCViettetlFC_Client.Models
{
    public class ShipmentResponse
    {
        public int code { get; set; }
        public string status { get; set; }
        public List<ShipmentData> data { get; set; }
    }

    public class ShipmentData
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public string carrier_name { get; set; }
        public string carrier_logo { get; set; }
        public string service_name { get; set; }
        public decimal delivery_fee { get; set; }
        public decimal service_fee { get; set; }
        public decimal amount_return_shop { get; set; }
        public decimal total_fee { get; set; }
        public string status_code { get; set; }
        public string status_text { get; set; }
        public string status_desc { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public address address_from { get; set; }
        public address address_to { get; set; }
        public parcel Parcel { get; set; }
        public string expected_delivery_date { get; set; }
        public string payer { get; set; }
        public string payer_txt { get; set; }
        public List<history> history { get; set; }
    }

    public class address
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string street { get; set; }
        public string district { get; set; }
        public string district_code { get; set; } // Added
        public string city { get; set; }
        public string city_code { get; set; } // Added
        public string ward { get; set; } // Added
        public string ward_code { get; set; } // Added
    }



    public class parcel
    {
        public string name { get; set; }
        public decimal weight { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal length { get; set; }
        public decimal cweight { get; set; } // Added
    }

    public class history
    {
        public int status { get; set; }
        public string status_text { get; set; }
        public string status_desc { get; set; }
        public DateTime updated_at { get; set; } // You may want to handle this as a Unix timestamp
    }
}
