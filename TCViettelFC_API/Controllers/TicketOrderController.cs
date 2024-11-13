using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.OrderTicket;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketOrderController : Controller
    {
        private readonly ITicketOrderRepository _ticketOrderRepository;
        private readonly ITicketUtilRepository _ticketUtilRepository;

        public TicketOrderController(ITicketOrderRepository ticketOrderRepository, ITicketUtilRepository ticketUtilRepository)
        {
            _ticketOrderRepository = ticketOrderRepository;
            _ticketUtilRepository = ticketUtilRepository;
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderedTicket([FromBody] TicketOrderDto ticketOrderDto, int? customerId = null)
        {
            if (ticketOrderDto == null)
            {
                return BadRequest("Order data is null.");
            }

            try
            {
                var obj = await _ticketOrderRepository.AddOderedTicket(ticketOrderDto, customerId);
                if (obj == null) return Conflict("Error add order");
                var result = await _ticketUtilRepository.SendTicketViaEmailAsync(obj.OrderId, obj.CustomerEmail);
                if (result == 1)
                    return Ok("Order added successfully.");
                return Conflict("Mail send error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
