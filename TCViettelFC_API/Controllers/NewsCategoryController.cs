using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsCategoryController : ControllerBase
    {
        private readonly ICategoryNewRepository _categoryNewRepository;

        public NewsCategoryController(ICategoryNewRepository categoryNewRepository)
        {
            _categoryNewRepository = categoryNewRepository;
        }
        [HttpGet("GetAllCategoryNews")]
        public async Task<IActionResult> GetAllCategoryNews()
        {
            var categories = await _categoryNewRepository.GetAllCategoryNewAsync();
            return Ok(categories);
        }
    }
}
