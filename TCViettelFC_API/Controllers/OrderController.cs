using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
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
        [HttpGet("getproductorders")]
        public async Task<IActionResult> GetAllOrderProducts()
        {
            var orders = await _orderRepository.GetAllOrderProductsAsync();
            return Ok(orders);
        }
        [HttpGet("getorderdetails/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var orderDetail = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
            if (orderDetail == null)
            {
                return NotFound($"Order details for order ID {orderId} not found.");
            }
            return Ok(orderDetail);
        }

        public class UpdateOrderStatusRequest
        {
            public int NewStatus { get; set; }
            public int StaffId { get; set; }
        }
        // Endpoint to update the order status
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderRepository.UpdateOrderStatusAsync(orderId, request.NewStatus, request.StaffId);

            if (!result)
            {
                return NotFound(new { Message = "Order not found" });
            }

            return Ok(new { Message = "Order status updated successfully" });
        }
        [HttpPost("shipment")]
        public async Task<IActionResult> UpsertShipment([FromBody] ShipmentDto shipmentDto)
        {
            if (shipmentDto.OrderId == null)
            {
                return BadRequest(new { Message = "OrderId is required" });
            }

            bool result = await _orderRepository.UpsertShipmentAsync(shipmentDto);

            if (result)
            {
                return Ok(new { Message = "Shipment added or updated successfully" });
            }

            return StatusCode(500, new { Message = "An error occurred while processing the shipment" });
        }

    }
}
