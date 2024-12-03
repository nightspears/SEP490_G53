
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewController : Controller
    {
        private readonly INewRepository _newRepository;
        public NewController(INewRepository newRepository)
        {
            _newRepository = newRepository;
        }
        [EnableQuery]
        [HttpGet("GetAllNews")]
        public IQueryable<GetNewDto> GetAllNews()
        {
            var newsList = _newRepository.GetAllNewsAsQueryable();

            return newsList;
        }
        [EnableQuery]
        [HttpGet("getallactivenews")]
        public async Task<IQueryable<GetNewDto>> GetAllActiveNews()
        {
            // Fetch the active news from the repository
            var newsList = await _newRepository.GetAllActiveNews();

            // Return the IQueryable, OData will handle pagination for you
            return newsList.AsQueryable();
        }
        // API get new by id
        [HttpGet("getnewsbyid/{id}")]
        public async Task<ActionResult<GetNewDto>> GetNewsById(int id)
        {
            var news = await _newRepository.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound(new { message = "News not found" });
            }
            return Ok(news);
        }
        [Authorize(Policy = "admin")]
        [HttpGet("updatestatus/{id}")]
        public async Task<IActionResult> UpdateNewsStatus(int id, int newStatus)
        {
            var result = await _newRepository.UpdateNewsStatusAsync(id, newStatus);

            if (!result)
            {
                return NotFound(new { message = "News not found" });
            }

            return Ok(new { message = "News status updated successfully" });
        }

        [Authorize(Policy = "staff")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateNews(CreateNewDto newDto)
        {
            try
            {
                var newId = await _newRepository.CreateNewsAsync(newDto);
                return Ok(new { message = "News created successfully", id = newId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create news", error = ex.Message });
            }
        }


        [Authorize(Policy = "staff")]
        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateNews(int id, UpdateNewDto newDto)
        {
            try
            {
                var isUpdated = await _newRepository.UpdateNewsAsync(id, newDto);
                if (!isUpdated)
                {
                    return NotFound(new { message = "News not found" });
                }

                return Ok(new { message = "News updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to update news", error = ex.Message });
            }
        }


        // Delete a news article
        [Authorize(Policy = "staff")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var result = await _newRepository.DeleteNewsAsync(id);
            if (!result)
            {
                return NotFound(new { message = "News not found" });
            }

            return Ok(new { message = "News deleted successfully" });
        }

    }
}
