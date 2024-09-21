using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public int? RoleId { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<NewsCategory> NewsCategories { get; set; } = new List<NewsCategory>();

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();

    public virtual Role? Role { get; set; }
}
