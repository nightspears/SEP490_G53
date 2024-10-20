namespace TCViettetlFC_Client.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string? CategoryName { get; set; }
        public string? SeasonName { get; set; }
        public string? Image { get; set; }
        public int? SeasonId { get; set; }
        public int? CategoryId { get; set; }

        public string? ProductName { get; set; }

        public IFormFile? Avatar { get; set; }

        public decimal? Price { get; set; }

        public string? Size { get; set; }

        public string? Color { get; set; }

        public string? Material { get; set; }

        public string? Description { get; set; }

        public int? discoutPercent { get; set; }

        public int? Status { get; set; }
        public List<FileResponse>? DataFile { get; set; }
        public List<int>? ListExist { get; set; }

    }
    public class FileResponse
    {
        public string? FileName { get; set; }

        public IFormFile? File { get; set; }
    }
    public class ProductFileViewModel
    {
        public int FileId { get; set; }

        public int? ProductId { get; set; }

        public string? FilePath { get; set; }

        public string? FileName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }

    }
    public class Player {

        public int PlayerId { get; set; }

        public string? FullName { get; set; }

        public int? ShirtNumber { get; set; }

        public int? SeasonId { get; set; }

        public string? Position { get; set; }

        public DateTime? JoinDate { get; set; }

        public DateTime? OutDate { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }
    }

    public class ApiResponse
    {
        public List<ProductFileViewModel> pFile { get; set; }
        public List<Player> players { get; set; }

        public ProductViewModel Product { get; set; }  
    }
}
