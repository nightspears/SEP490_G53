namespace TCViettetlFC_Client.Models
{
    public class CheckoutModel
    {

        public List<CheckoutCartModel> checkoutItems { get; set; } = new List<CheckoutCartModel>();

        public string Email { get; set; }
        public int? AccountId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string WardId { get; set; }
        public string WardName { get; set; }
        public string ?Notes { get; set; }
        public int TotalAmount { get; set; }
        public string SelectedShipping { get; set; }
    }
  

    public class CheckoutCartModel
    {
        public int ProductId { get; set; }
        public string nameProduct { get; set; }
        public decimal Price { get; set; }
        public string size { get; set; }
        public string Avartar { get; set; }
        public int Quantity { get; set; }
        public int shirtNumber { get; set; }
        public string ?SoAo { get; set; }
        public string ?TenCauThu { get; set; }



    }
}
