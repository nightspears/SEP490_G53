using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Dtos.MatchAreaTicket;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchAreasController : ControllerBase
    {
        private readonly IMatchAreaRepository matchArea;

        public MatchAreasController(IMatchAreaRepository _matchArea)
        {
            matchArea = _matchArea;

        }

        [HttpGet("GetMatchArea")]
        public async Task<ActionResult<IEnumerable<MatchAreaTicketDto>>> GetMatches()
        {
            var match = await matchArea.GetMatchAreaAsync();
            return Ok(match);

        }
        [EnableQuery]
        [HttpGet("GetSanPhamById")]
        public async Task<ActionResult<IEnumerable<MatchAreaRespone>>> GetSanPhamById(int id)
        {
            var match = await matchArea.GetSanPhamById(id);
            return Ok(match);

        }
        [HttpPut("UpdateSeat")]
        public  ActionResult UpdateSeat([FromBody] MatchAreaRequest matchAreaDto)
        {
            
            matchArea.UpdateSeat(matchAreaDto);
            return Ok();

        }

        

    }
}
