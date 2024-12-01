using Microsoft.AspNetCore.Mvc;

namespace TCViettelFC_Client.Controllers.Admin
{
    public class TeamManagementController : Controller
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
