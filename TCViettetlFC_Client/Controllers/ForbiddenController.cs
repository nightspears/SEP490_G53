using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers
{
    public class ForbiddenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
