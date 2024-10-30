namespace TCViettelFC_API.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? AccountId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? FullName { get; set; }

    public int? Status { get; set; }

    public virtual CustomersAccount? Account { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<TicketOrder> TicketOrders { get; set; } = new List<TicketOrder>();
}
