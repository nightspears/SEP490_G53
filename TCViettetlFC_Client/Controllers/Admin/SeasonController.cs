using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers.Admin
{
    public class SeasonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
