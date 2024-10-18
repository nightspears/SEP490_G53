using TCViettelFC_API.Models;

namespace TCViettelFC_API.Dtos.Product
{
    public class ProductDto 
    {

        //public int ProductId { get; set; }

        public int? PlayerId { get; set; }

        public int? SeasonId { get; set; }

        public int? CategoryId { get; set; }

        public string? ProductName { get; set; }

        public IFormFile? Avatar { get; set; }

        public decimal? Price { get; set; }

        public string? Size { get; set; }

        public string? Color { get; set; }

        public string? Material { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }
        public List<FileResponse>? DataFile { get; set; }
        public List<int>? ListExist { get; set; }
    }

    public class FileResponse
    {
        public string? FileName { get; set; }

        public IFormFile? File { get; set; }
    }

    public class ProductResponse : ProductDto
    {
        public int ProductId { get; set; }
        public string? CategoryName{ get; set; }
        public string? SeasonName { get; set; }
        public string? Image { get; set; }

    }

    public class ProductCart {

        public int ProductId { get; set; }

        public int? CategoryId { get; set; }

        public string? ProductName { get; set; }

        public string? Avatar { get; set; }

        public decimal? Price { get; set; }

        public string? Size { get; set; }

        public string? Material { get; set; }

        public string? Description { get; set; }
        public int? Quantity{ get; set; }

    }




}
