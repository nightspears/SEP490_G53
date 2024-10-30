namespace TCViettelFC_API.Models;

public partial class NewsCategory
{
    public int Id { get; set; }

    public int? CreatorId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual User? Creator { get; set; }

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
