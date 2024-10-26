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

        public TicketOrderController(ITicketOrderRepository ticketOrderRepository )
        {
            _ticketOrderRepository = ticketOrderRepository;
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
                await _ticketOrderRepository.AddOderedTicket(ticketOrderDto, customerId);
                return Ok("Order added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
