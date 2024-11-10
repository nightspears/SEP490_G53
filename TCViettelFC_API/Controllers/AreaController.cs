using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepository _areaRepository;
        public AreaController(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
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
    }
}
