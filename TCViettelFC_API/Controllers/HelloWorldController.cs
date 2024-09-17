using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        private readonly IHelloWorldRepository _helloWorldRepository;
        public HelloWorldController(IHelloWorldRepository helloWorldRepository)
        {
            _helloWorldRepository = helloWorldRepository;
        }
        [HttpGet]
        public IActionResult GetString()
        {
            var message = _helloWorldRepository.GetString();
            return Ok(new { Message = $"{message}" });
        }

    }
}
