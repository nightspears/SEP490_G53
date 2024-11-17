namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<int> GetTotalCustomer();
        Task<decimal?> GetTotalTicketRevenue();
        Task<decimal?> GetTotalProductRevenue();
        Task<int> GetTotalSoldTickets();

    }
}
