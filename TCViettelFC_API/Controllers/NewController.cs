
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
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

        [HttpGet("GetAllNews")]
        public async Task<ActionResult<List<GetNewDto>>> GetAllNews()
        {
            var newsList = await _newRepository.GetAllNewsAsync();
            if (newsList == null || newsList.Count == 0)
            {
                return NotFound("No news found.");
            }
            return Ok(newsList);
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
        

    }
}
