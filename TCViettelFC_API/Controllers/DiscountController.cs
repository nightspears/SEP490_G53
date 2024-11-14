using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Discount;
using TCViettelFC_API.Dtos.Season;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {

        private readonly IDiscountRepository _discount;

        public DiscountController(IDiscountRepository discount)
        {
            _discount = discount;

        }
       

        [HttpGet("GetDiscount")]
        public async Task<ActionResult<List<Discount>>> GetDiscountAsync()
        {
            try
            {
                var dis = await _discount.GetDiscountAsync();
                return Ok(dis);
            }
            catch (Exception ex) { 
                 return BadRequest(ex.Message);
            }
            

        }
        [HttpGet("GetDiscountById")]
        public async Task<ActionResult> GetDiscountById(int id)
        {
            try
            {
                var discount = await _discount.GetDiscountByIdAsync(id);
                return Ok(discount);


            }
            catch (Exception ex)
            {
                return BadRequest("Không tìm thấy season");
            }


        }
        [HttpPost("AddDiscount")]
        public async Task<IActionResult> AddDiscountAsync([FromForm] DiscountDto discount)
        {
            try
            {
                await _discount.AddDiscountAsync(discount);
                return Ok("Thêm mã giảm giá thành công");
            }
            catch (Exception ex) { 
                   return Conflict(ex);
            }

           
        }

        [HttpPut("UpdateDiscount/{id}")]
        public async Task<IActionResult> UpdateSeasonAsync(int id, [FromForm]  DiscountDto discount)
        {
            try
            {

                await _discount.UpdateDiscountAsync(id, discount);
                return Ok("Cập nhật mã giảm giá thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi update: " + ex.Message);
            }
        }
        [HttpDelete("DeleteDiscount/{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            try
            {
                await _discount.DeleteDiscountAsync(id);
                return Ok("Xóa mã giảm giá thành công.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tim thấy mã giảm giá .");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa lỗi : " + ex.Message);
            }
        }
    }
}
