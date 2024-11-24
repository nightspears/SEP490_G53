using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;

        public MatchesController(IMatchRepository matchRepository )
        {
            _matchRepository = matchRepository;

        }
        [HttpGet("GetMatches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatches()
        {
            var match = await _matchRepository.GetMatchesAsync();
            var data = new
            {
                data = match,
            };
            return Ok(match);

        }
        [HttpGet("GetMatchesById")]
        public async Task<ActionResult> GetMatchesById(int id)
        {
            var match = await _matchRepository.GetMatchesByIdAsync(id);
            
            return Ok(match);

        }
        [HttpPost("AddMatches")]
        public async Task<IActionResult> AddMatchesAsync([FromForm] MatchesAddDto matchDto)
        {
            await _matchRepository.AddMatchesAsync(matchDto);
            return Ok("Thêm trận thành công");
        }

        [HttpPut("UpdateMatches/{id}")]
        public async Task<IActionResult> UpdateMatchAsync(int id, [FromForm] MatchesAddDto matchDto)
        {
            try
            {
              
                await _matchRepository.UpdateMatchesAsync(id, matchDto);
                return Ok("Cập nhật trận đấu thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi update: " + ex.Message);
            }
        }
        [HttpDelete("DeleteMatches/{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            try
            {
                await _matchRepository.DeleteMatchesAsync(id);
                return Ok("Xóa trận đấu thành công.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không thấy trận đấu .");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa lỗi : " + ex.Message);
            }
        }

        [HttpPost("updateStatus")]
        public void updateStatus()
        {
            var status = int.Parse(Request.Form["status"]);
            var id = int.Parse(Request.Form["id"]);
            _matchRepository.UpdateStatus(status, id);
        }
    }
}
