namespace TCViettelFC_API.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
