namespace TCViettelFC_API.Models;

public partial class ProductFile
{
    public int FileId { get; set; }

    public int? ProductId { get; set; }

    public string? FilePath { get; set; }

    public string? FileName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual Product? Product { get; set; }
}
