using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [Authorize(Policy = "staff")]
        [HttpGet("getticketorders")]
        public async Task<IActionResult> GetAllTicketOrders()
        {
            return Ok(await _orderRepository.GetAllTicketOrders());
        }
        [Authorize(Policy = "staff")]
        [HttpGet("getorderedticket/{id}")]
        public async Task<IActionResult> GetOrderedTicketByOrderedId(int id)
        {
            return Ok(await _orderRepository.GetOrderedTicketByTicketOrderId(id));
        }
        [Authorize(Policy = "staff")]
        [HttpGet("getorderedsupp/{id}")]
        public async Task<IActionResult> GetOrderedSuppByOrderId(int id)
        {
            return Ok(await _orderRepository.GetOrderedSuppItemByTicketOrderId(id));
        }

    }
}
