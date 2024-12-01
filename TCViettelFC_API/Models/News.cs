using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class News
{
    public int Id { get; set; }

    public int? CreatorId { get; set; }

    public int? NewsCategoryId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Image { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual User? Creator { get; set; }

    public virtual NewsCategory? NewsCategory { get; set; }
}
