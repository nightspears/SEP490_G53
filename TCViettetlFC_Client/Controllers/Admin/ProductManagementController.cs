using Microsoft.AspNetCore.Mvc;

namespace TCViettetlFC_Client.Controllers.Admin
{
    public class ProductManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
