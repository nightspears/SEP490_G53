using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly Sep490G53Context _context;
        public AdminDashboardRepository(Sep490G53Context context)
        {
            _context = context;
        }
        public async Task<int> GetTotalCustomer()
        {
            var customeracc = await _context.CustomersAccounts.CountAsync();
            var customer = await _context.Customers.Where(x => x.AccountId == null).CountAsync();
            return customeracc + customer;
        }
        public async Task<decimal?> GetTotalTicketRevenue()
        {
            return await _context.TicketOrders.Select(x => x.TotalAmount).SumAsync();
        }
        public async Task<decimal?> GetTotalProductRevenue()
        {
            return await _context.OrderProducts.Select(x => x.TotalPrice).SumAsync();
        }
        public async Task<int> GetTotalSoldTickets()
        {
            return await _context.OrderedTickets.CountAsync();
        }
    }
}
