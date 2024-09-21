﻿using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
