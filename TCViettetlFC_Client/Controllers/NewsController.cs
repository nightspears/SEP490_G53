using Microsoft.AspNetCore.Mvc;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class NewsController : Controller
    {
        private readonly int itemPerPage = 8;
        private IApiHelper _apiHelper;
        public NewsController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        private async Task<List<CustomerNewsModel>> GetAllNews(int page)
        {
            return await _apiHelper.GetApiResponseAsync<List<CustomerNewsModel>>($"New/getallactivenews?$skip={(page - 1) * itemPerPage}&$top={itemPerPage}");
        }
        private async Task<CustomerNewsModel> GetNewsByNewsId(int id)
        {
            return await _apiHelper.GetApiResponseAsync<CustomerNewsModel>($"new/getnewsbyid/{id}");
        }

        public async Task<IActionResult> Index(int id = 1)
        {
            var result = await _apiHelper.GetApiResponseAsync<List<CustomerNewsModel>>($"New/getallactivenews");
            int totalPage = (int)Math.Ceiling((double)result.Count() / itemPerPage);
            if (id > totalPage) id = totalPage;
            if (id < 1) id = 1;
            TempData["TotalPages"] = totalPage;
            var news = await GetAllNews(id);
            return View(news);
        }
        public async Task<IActionResult> Details(int id)
        {
            var news = await GetNewsByNewsId(id);
            if (news == null) return RedirectToAction("Index");
            return View(news);
        }
    }
}
