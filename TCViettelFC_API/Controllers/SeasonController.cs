using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Season;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController : ControllerBase
    {

        private readonly ISeasonRepository _season;

        public SeasonController(ISeasonRepository season)
        {
            _season = season;

        }
       

        [HttpGet("GetSeason")]
        public async Task<ActionResult<List<SeasonResponse>>> GetSeasonAsync()
        {
            var cate = await _season.GetSeasonAsync();
            return Ok(cate);

        }
        [HttpGet("GetSeasonById")]
        public async Task<ActionResult> GetSeasonById(int id)
        {
            try
            {
                var season = await _season.GetSeasonByIdAsync(id);
                return Ok(season);


            }
            catch (Exception ex)
            {
                return BadRequest("Không tìm thấy season");
            }


        }
        [HttpPost("AddSeason")]
        public async Task<IActionResult> AddSeasonAsync([FromForm] SeasonDto season)
        {
            await _season.AddSeasonAsync(season);
            return Ok("Thêm mùa giải thành công");
        }

        [HttpPut("UpdateSeason/{id}")]
        public async Task<IActionResult> UpdateSeasonAsync(int id, [FromForm]  SeasonDto season)
        {
            try
            {

                await _season.UpdateSeasonAsync(id, season);
                return Ok("Cập nhật season thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi update: " + ex.Message);
            }
        }
        [HttpDelete("DeleteSeason/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _season.DeleteSeasonAsync(id);
                return Ok("Xóa season thành công.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tim thấy Season .");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa lỗi : " + ex.Message);
            }
        }
    }
}
