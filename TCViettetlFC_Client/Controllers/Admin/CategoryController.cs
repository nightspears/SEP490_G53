using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers.Admin
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
    }
}
