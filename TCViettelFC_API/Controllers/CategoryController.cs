using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;

using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryRepository _category;

        public CategoryController(ICategoryRepository category)
        {
            _category = category;

        }
       

        [HttpGet("GetCategory")]
        public async Task<ActionResult<List<ProductCategory>>> GetCategory()
        {
            var cate = await _category.GetCateAsync();
            return Ok(cate);

        }
        [HttpGet("GetCateById")]
        public async Task<ActionResult> GetCateById(int id)
        {
            var match = await _category.GetCateByIdAsync(id);

            return Ok(match);

        }
        [HttpPost("AddCate")]
        public async Task<IActionResult> AddCateAsync([FromForm]  CategoryDto category)
        {
            await _category.AddCateAsync(category);
            return Ok("Thêm category thành công");
        }

        [HttpPut("UpdateCate/{id}")]
        public async Task<IActionResult> UpdatecCateAsync(int id, [FromForm] CategoryDto category)
        {
            try
            {

                await _category.UpdateCateAsync(id, category);
                return Ok("Cập nhật category thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi update: " + ex.Message);
            }
        }
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _category.DeleteCateAsync(id);
                return Ok("Xóa category thành công.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tim thấy Category .");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa lỗi : " + ex.Message);
            }
        }
    }
}
