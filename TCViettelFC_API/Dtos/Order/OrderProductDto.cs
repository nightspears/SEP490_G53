using TCViettelFC_API.Dtos.CheckOut;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Dtos.Order
{
    public class OrderProductDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? OrderCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? AddressId { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }

        public string? Email { get; set; }  
        public string? Phone { get; set; }
        public string? FullName { get; set; }
    }


    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string? OrderCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? ShipmentFee { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }

        // Customer Information
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerFullName { get; set; }

        // Address Information
        public AddressDto Address { get; set; } = new AddressDto();

        // Product Details
        public List<ProductDetailDto> Products { get; set; } = new List<ProductDetailDto>();

        // Payment Information
        public PaymentDto Payment { get; set; } = new PaymentDto();
    }



    public class AddressDto
    {
        public int AddressId { get; set; }
        public int? CustomerId { get; set; }
        public string? CityName { get; set; }
        public string? City { get; set; }
        public string? DistrictName { get; set; }
        public string? District { get; set; }
        public string? WardName { get; set; }
        public string? Ward { get; set; }
        public string? DetailedAddress { get; set; }
        public int? Status { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public int? OrderTicketId { get; set; }
        public int? OrderProductId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PaymentGateway { get; set; }
        public int? Status { get; set; }
    }

    public class ProductDetailDto
    {
        public int Id { get; set; }
        public int? OrderProductId { get; set; }
        public ProductorderDto Product { get; set; } = new ProductorderDto(); 
        public PlayerProductorderDto Player { get; set; } = new PlayerProductorderDto(); 
        public string? CustomShirtNumber { get; set; }
        public string? CustomShirtName { get; set; }
        public string? Size { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
    }
    public class ProductorderDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Avatar { get; set; }
    }
    public class PlayerProductorderDto
    {
        public int PlayerId { get; set; }

        public string? FullName { get; set; }

        public int? ShirtNumber { get; set; }

    }
}


