using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepository _areaRepository;
        private readonly Sep490G53Context _context;
        public AreaController(IAreaRepository areaRepository, Sep490G53Context context)
        {
            _areaRepository = areaRepository;
            _context = context;
        }

        [HttpGet("getareabyid/{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var result = await _areaRepository.GetAreaById(id);
            if (result == null) return NotFound("Không tìm thấy khu vực với id này");
            return Ok(result);
        }

        [HttpGet("getmatchareaticketsbymatchid/{matchId}")]
        public async Task<IActionResult> GetMatchAreaTicketsByMatchId(int matchId)
        {
            var tickets = await _areaRepository.GetMatchAreaTicketsByMatchIdAsync(matchId);
            if (tickets == null || tickets.Count == 0) return NotFound("Không tìm thấy vé khu vực cho trận đấu này");
            return Ok(tickets);
        }

       

        [HttpPut("increasecount/{matchId}/{areaId}")]
        public async Task<IActionResult> IncreaseCount(int matchId, int areaId)
        {
            // Find the MatchAreaTicket by MatchId and AreaId
            var matchAreaTicket = await _context.MatchAreaTickets
                .FirstOrDefaultAsync(mat => mat.MatchId == matchId && mat.AreaId == areaId);

            if (matchAreaTicket == null)
            {
                return NotFound("Không tìm thấy vé khu vực cho trận đấu và khu vực này.");
            }

            // Check if AvailableSeats is greater than the current Count
            if (matchAreaTicket.AvailableSeats.HasValue && matchAreaTicket.Count.HasValue)
            {
                if (matchAreaTicket.AvailableSeats.Value <= matchAreaTicket.Count.Value)
                {
                    return BadRequest("Ghế đã được giữ chỗ, bạn vui lòng thử lại sau");
                }
            }

            // Increment the Count by 1
            matchAreaTicket.Count = (matchAreaTicket.Count ?? 0) + 1;

            // Save changes to the database
            _context.MatchAreaTickets.Update(matchAreaTicket);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Count increased by 1" });
        }


        [HttpPut("decreasecount/{matchId}/{areaId}/{decreaseAmount}")]
        public async Task<IActionResult> DecreaseCount(int matchId, int areaId, int decreaseAmount)
        {
            // Find the MatchAreaTicket by MatchId and AreaId
            var matchAreaTicket = await _context.MatchAreaTickets
                .FirstOrDefaultAsync(mat => mat.MatchId == matchId && mat.AreaId == areaId);

            if (matchAreaTicket == null)
            {
                return NotFound("Không tìm thấy vé khu vực cho trận đấu và khu vực này.");
            }

            // Decrement the Count by the specified amount
            if (matchAreaTicket.Count.HasValue && matchAreaTicket.Count.Value >= decreaseAmount)
            {
                matchAreaTicket.Count -= decreaseAmount;

                // Save changes to the database
                _context.MatchAreaTickets.Update(matchAreaTicket);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Count decreased by {decreaseAmount}" });
            }
            else
            {
                return BadRequest("Invalid decrease amount or insufficient count.");
            }
        }




    }
}
