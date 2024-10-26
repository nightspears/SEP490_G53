using TCViettelFC_API.Models;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICheckoutRepository
    {
        Task<bool> InsertCheckoutDataAsync(Customer customer, Address address, OrderProduct orderProduct, List<OrderProductDetail> orderProductDetails, Payment payment);
        Task SaveAddressAsync(Address address);
        Task SaveCustomerAsync(Customer customer);
        Task SaveOrderProductAsync(OrderProduct orderProduct);
    }
}

