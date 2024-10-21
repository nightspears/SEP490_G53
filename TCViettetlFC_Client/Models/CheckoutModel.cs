namespace TCViettetlFC_Client.Models
{
    public class CheckoutModel
    {

        public List<CheckoutCartModel> checkoutItems { get; set; } = new List<CheckoutCartModel>();

        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
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
        public int playerId { get; set; }

    }
}
