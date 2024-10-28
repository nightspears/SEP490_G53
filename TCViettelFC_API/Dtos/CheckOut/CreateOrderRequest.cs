namespace TCViettelFC_API.Dtos.CheckOut
{
    public class CreateOrderRequest
    {
        public CustomerDTO Customer { get; set; }
        public AddressDTO Address { get; set; }
        public OrderProductDTO OrderProduct { get; set; }
        public List<OrderProductDetailDTO> OrderProductDetails { get; set; }
        public PaymentDTO Payment { get; set; }
    }

    public class CustomerDTO
    {
        public int? AccountId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? FullName { get; set; }
    }

    public class AddressDTO
    {
        public string City { get; set; }
        public string CityName { get; set; }
        public string District { get; set; }
        public string DistrictName { get; set; }
        public string Ward { get; set; }
        public string WardName { get; set; }
        public string DetailedAddress { get; set; }
    }

    public class OrderProductDTO
    {
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderProductDetailDTO
    {
        public int ProductId { get; set; }
        public int? PlayerId { get; set; }
        public string? CustomShirtNumber { get; set; }

        public string? CustomShirtName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class PaymentDTO
    {
        public decimal TotalAmount { get; set; }
        public string PaymentGateway { get; set; }
    }
}
