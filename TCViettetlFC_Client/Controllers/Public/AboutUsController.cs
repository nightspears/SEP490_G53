using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers.Public
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
