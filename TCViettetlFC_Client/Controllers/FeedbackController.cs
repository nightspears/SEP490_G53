using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly HttpClient _httpClient;
        public FeedbackController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");

        }
        public IActionResult Index()
        {
            var token = Request.Cookies["CustomerId"];
            if (token == null)
            {
                return RedirectToAction("Login", "Customer");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddFeedback(AddFeedbackViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ModelState.AddModelError("Content", "Nội dung đánh giá không được để trống.");
                return View(model);
            }
            var result = await _httpClient.PostAsJsonAsync("feedback/addfeedback", model);
            if (result.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi thành công!";
                return RedirectToAction("FeedbackConfirmation");
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gửi đánh giá của bạn.");
                return View(model);
            }
        }
        public IActionResult FeedbackConfirmation()
        {
            return View();
        }

    }
}
