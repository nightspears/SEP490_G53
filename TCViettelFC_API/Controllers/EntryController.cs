using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly ITicketUtilRepository _ticketUtilRepository;
        public EntryController(ITicketUtilRepository ticketUtilRepository)
        {
            _ticketUtilRepository = ticketUtilRepository;
        }
        [Authorize(Policy = "entry")]
        [HttpGet("verifyticket/{ticketId}")]
        public async Task<IActionResult> VerifyTicketAsync(int ticketId)
        {
            var ticket = await _ticketUtilRepository.GetOrderedTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound("Ticket not found");
            var result = await _ticketUtilRepository.VerifyTicketAsync(ticket);
            if (result == 1) return Ok("Ticket verified successfully");
            return BadRequest("Ticket not valid");
        }
        [Authorize(Policy = "entry")]
        [HttpGet("verifysupitem/{orderId}")]
        public async Task<IActionResult> VerifyItemAsync(int orderId)
        {
            var items = await _ticketUtilRepository.GetOrderedSuppByOrderId(orderId);
            if (items == null) return NotFound("Không tìm thấy sản phẩm đi kèm nào");
            var result = await _ticketUtilRepository.VerifyItemAsync(items);
            if (result.Count > 0) return Ok(result);
            return BadRequest("Sản phẩm đã nhận");
        }
    }
}
