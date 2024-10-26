using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.TiketOrder;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketOrderController : Controller
    {
        private readonly ITiketOrderRepository _repository;
        public TicketOrderController(ITiketOrderRepository repository)
        {
            _repository = repository;
        }
        [HttpPost("add-to-cart")]
        public IActionResult AddTicketsToCart([FromBody] TicketOrderCreateDto ticketOrder)
        {
            if (ticketOrder == null || ticketOrder.Tickets.Count == 0)
            {
                return BadRequest("No tickets specified.");
            }

            var order = _repository.CreateOrUpdateOrder(ticketOrder.CustomerId);

            foreach (var ticket in ticketOrder.Tickets)
            {
                if (!_repository.CheckTicketAvailabilityAndUpdate(ticket.MatchId, ticket.AreaId, ticket.Quantity))
                {
                    return BadRequest($"Not enough tickets available for match {ticket.MatchId} in area {ticket.AreaId}.");
                }
                _repository.AddTicketToOrder(order.Id, ticket);
            }

            // Thêm mặt hàng bổ sung nếu có
            foreach (var item in ticketOrder.SupplementaryItems)  // Giả sử bạn đã thêm một danh sách các mặt hàng bổ sung trong DTO
            {
                _repository.AddSupplementaryItemToOrder(order.Id, item);
            }

            _repository.SaveChanges();

            return Ok(new { orderId = order.Id });
        }
    }
}

