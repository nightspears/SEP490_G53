using Microsoft.AspNetCore.Mvc;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class NewsController : Controller
    {
        private IApiHelper _apiHelper;
        public NewsController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        private async Task<List<CustomerNewsModel>> GetAllNews()
        {
            return await _apiHelper.GetApiResponseAsync<List<CustomerNewsModel>>("New/GetAllNews");
        }
        private async Task<CustomerNewsModel> GetNewsByNewsId(int id)
        {
            return await _apiHelper.GetApiResponseAsync<CustomerNewsModel>($"new/getnewsbyid/{id}");
        }
        public async Task<IActionResult> Index()
        {
            var news = await GetAllNews();
            return View(news);
        }
        [HttpGet("News/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var news = await GetNewsByNewsId(id);
            if (news == null) return RedirectToAction("Index");
            return View(news);
        }
    }
}
