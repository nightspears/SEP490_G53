using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public int? RoleId { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<NewsCategory> NewsCategories { get; set; } = new List<NewsCategory>();

    public virtual Role? Role { get; set; }
}
