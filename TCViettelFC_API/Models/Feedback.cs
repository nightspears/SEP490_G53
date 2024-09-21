using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual User? User { get; set; }
}
