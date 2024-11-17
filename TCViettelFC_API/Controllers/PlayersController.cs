using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepository _player;

        public PlayersController(IPlayerRepository player)
        {
            _player = player;
        }
        [EnableQuery]
        [HttpPost("AddPlayer")]
        public async Task<IActionResult> AddPlayer(PlayerDto playerDto)
        {
            try
            {
                var createdPlayer = await _player.AddPlayerAsync(playerDto);
                return CreatedAtAction(nameof(GetPlayerById), new { id = createdPlayer.PlayerId }, createdPlayer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [EnableQuery]
        [HttpGet("ListPlayer")]
        public async Task<IActionResult> GetAllPlayers()
        {
            try
            {
                var players = await _player.ListAllPlayerAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [EnableQuery]
        [HttpGet("GetPlayerById")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            try
            {
                var player = await _player.GetPlayerByIdAsync(id);
                return Ok(player);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [EnableQuery]
        [HttpPut("UpdatePlayer/{id}")]
        public async Task<IActionResult> UpdatePlayer(int id,[FromBody] PlayerDto playerDto)
        {
            try
            {
                var updatedPlayer = await _player.UpdatePlayerAsync(id,playerDto);
                return Ok(updatedPlayer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [EnableQuery]
        [HttpDelete("DeletePlayer")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            try
            {
                var deletedPlayer = await _player.DeletePlayerAsync(id);
                return Ok(deletedPlayer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
