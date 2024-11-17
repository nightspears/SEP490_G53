using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _product;

        public ProductController(IProductRepository pro)
        {
            _product = pro;

        }
        [HttpGet("GetProduct")]
        public async Task<ActionResult<List<ProductResponse>>> GetProduct()
        {
            try
            {
                var product = await _product.GetProductAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình thực thi");
            }


        }
        [EnableQuery]
        [HttpGet("GetSanPham")]
        public async Task<ActionResult<List<ProductResponse>>> GetSanPham(int cid)
        {
            try
            {
                List<ProductResponse> product = await _product.GetSanPhamAsync(cid);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình thực thi");
            }


        }
        [HttpGet("GetProductById")]
        public async Task<JsonResult> GetProductById(int id)
        {

            try
            {
                var data = await _product.GetProductByIdAsync(id);
                return data;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong quá trình thực thi.");
            }


        }

        [HttpGet("GetSanPhamById")]
        public async Task<JsonResult> GetSanPhamById(int id)
        {

            try
            {
                var data = await _product.GetSanPhamByIdAsync(id);
                return data;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong quá trình thực thi.");
            }


        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProductAsync(ProductDto product)
        {
            try
            {
                await _product.AddProductAsync(product);
                return Ok("Thêm product thành công");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi thêm sản phẩm");
            }

        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdatecProductAsync(int id, ProductDto product)
        {
            try
            {

                await _product.UpdateProductAsync(id, product);
                return Ok("Cập nhật product thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi update: " + ex.Message);
            }
        }
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _product.DeleteProductAsync(id);
                return Ok("Xóa product thành công.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tim thấy Product .");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa lỗi : " + ex.Message);
            }
        }

        [HttpGet("GetJson")]
        public async Task<JsonResult> GetDataFill()
        {
            var data = await _product.GetDataJsonAsync();
            return data;

        }

        [HttpPost("SanPhamLienQuan")]
        public async Task<JsonResult> SanPhamLienQuan([FromBody] List<int> lstID)
        {
            var data = await _product.GetLienQuanProductAsync(lstID);
            return data;

        }

        [HttpPost("updateStatus")]
        public void updateStatus()
        {
            var status = int.Parse( Request.Form["status"] );
            var id = int.Parse(Request.Form["id"]);
            _product.UpdateStatus(status , id);
        }
    }
}
