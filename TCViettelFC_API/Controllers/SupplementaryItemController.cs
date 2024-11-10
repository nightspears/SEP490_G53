using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;
using TCViettelFC_API.Models;
using TCViettelFC_API.Dtos.Supplementary;
using TCViettelFC_API.Repositories.Implementations;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplementaryItemController : ControllerBase
    {
        private readonly ISupplementaryItemRepository _repository;

        public SupplementaryItemController(ISupplementaryItemRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                // Retrieve all supplementary items from the repository
                var items = await _repository.GetAllAsync();

                if (items == null || !items.Any())
                {
                    return NotFound(new { message = "No supplementary items found." });
                }

                // Return the list of items
                return Ok(items);
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur
                return StatusCode(500, new { message = "An error occurred while retrieving the items.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] SupplementaryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdItem = await _repository.CreateAsync(dto);
            return CreatedAtAction(nameof(GetItemById), new { id = createdItem.ItemId }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] SupplementaryDto dto)
        {
            if (id != dto.ItemId)
                return BadRequest();

            await _repository.UpdateAsync(id, dto);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
