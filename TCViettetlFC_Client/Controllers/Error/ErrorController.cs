using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers.Error
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("Error/403")]
        public IActionResult Error403()
        {
            return View();
        }

        [Route("Error/400")]
        public IActionResult Error400()
        {
            return View();
        }
    }
}
