using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public int? CreatorId { get; set; }

    public int? ResponderId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public virtual Customer? Creator { get; set; }

    public virtual User? Responder { get; set; }
}
