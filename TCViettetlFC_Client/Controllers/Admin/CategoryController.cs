using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers.Admin
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
