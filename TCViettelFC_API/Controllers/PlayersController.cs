using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayersController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult> GetAllPlayers()
        {
            try
            {
                var players = await _playerRepository.ListAllPlayerAsync();
                if (players == null || players.Count == 0)
                    return NotFound("No players found.");
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving players: {ex.Message}");
            }
        }

        // GET: api/Players/active
        [HttpGet("active")]
        public async Task<ActionResult> GetAllPlayersActive()
        {
            try
            {
                var players = await _playerRepository.ListAllPlayerActiveAsync();
                if (players == null || players.Count == 0)
                    return NotFound("No active players found.");
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving active players: {ex.Message}");
            }
        }

        // GET: api/Players/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPlayerById(int id)
        {
            try
            {
                var player = await _playerRepository.GetPlayerByIdAsync(id);
                if (player == null)
                    return NotFound($"Player with ID {id} not found.");
                return Ok(player);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving player by ID: {ex.Message}");
            }
        }

        // POST: api/Players
        [HttpPost]
        public async Task<ActionResult> AddPlayer(PlayerInputDto playerInputDto)
        {
            if (playerInputDto == null)
                return BadRequest("Player data cannot be null.");

            var playerDto = new PlayerDto
            {
                FullName = playerInputDto.FullName,
                ShirtNumber = playerInputDto.ShirtNumber,
                Position = playerInputDto.Position,
                JoinDate = playerInputDto.JoinDate,
                OutDate = playerInputDto.OutDate,
                Description = playerInputDto.Description,
                Status = playerInputDto.Status,
                SeasonId = playerInputDto.SeasonId,
                Avatar = playerInputDto.Avatar
            };

            try
            {
                var newPlayer = await _playerRepository.AddPlayerAsync(playerDto);
                return CreatedAtAction(nameof(GetPlayerById), new { id = newPlayer.PlayerId }, newPlayer);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding player: {ex.Message}");
            }
        }

        // PUT: api/Players/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePlayer(int id, PlayerInputDto playerInputDto)
        {
            if (playerInputDto == null)
                return BadRequest("Player data cannot be null.");
            if (playerInputDto.SeasonId <= 0)
                return BadRequest("Invalid season ID.");
            var playerDto = new PlayerDto
            {
                FullName = playerInputDto.FullName,
                ShirtNumber = playerInputDto.ShirtNumber,
                Position = playerInputDto.Position,
                JoinDate = playerInputDto.JoinDate,
                OutDate = playerInputDto.OutDate,
                Description = playerInputDto.Description,
                Status = playerInputDto.Status,
                SeasonId = playerInputDto.SeasonId,
                Avatar = playerInputDto.Avatar
            };

            try
            {
                var updatedPlayer = await _playerRepository.UpdatePlayerAsync(id, playerDto);
                return Ok(updatedPlayer);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating player: {ex.Message}");
            }
        }

        // DELETE: api/Players/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayer(int id)
        {
            try
            {
                var deletedPlayer = await _playerRepository.DeletePlayerAsync(id);
                return Ok(deletedPlayer);
            }
            catch (Exception ex)
            {
                return NotFound($"Error deleting player: {ex.Message}");
            }
        }
    }
}
