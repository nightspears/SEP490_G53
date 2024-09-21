using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class NewsCategory
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual User? User { get; set; }
}
