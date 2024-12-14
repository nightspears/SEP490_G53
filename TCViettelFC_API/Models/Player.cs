namespace TCViettelFC_API.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string? FullName { get; set; }

    public int? ShirtNumber { get; set; }

    public int? SeasonId { get; set; }

    public string? Position { get; set; }

    public DateTime? JoinDate { get; set; }

    public DateTime? OutDate { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public string? avatar { get; set; }

    public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; } = new List<OrderProductDetail>();

    public virtual Season? Season { get; set; }
}
