using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using TCViettetlFC_Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;


namespace TCViettetlFC_Client.Controllers.Common
{
    public class PublicController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly FeedbackService _feedbackService;
        private readonly IApiHelper _apiHelper;

        public PublicController(IHttpClientFactory httpClientFactory, FeedbackService feedbackService, IApiHelper apiHelper)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _feedbackService = feedbackService;
            _apiHelper = apiHelper;
        }
        public IActionResult SanPham()
        {
            return View();
        }
      
        public async Task<IActionResult> ChiTietSanPham(int id)
        {
            ViewBag.Id = id;

            string requestUri = "https://localhost:5000/api/Product/GetSanPhamById?id=" + id;
            var response = await _httpClient.GetAsync(requestUri);

           ApiResponse ListData = new ApiResponse();

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                ListData = JsonConvert.DeserializeObject<ApiResponse>(jsonData);
            }
          
            return View(ListData);
        }


        public IActionResult GioHang()
        {
            return View();
        }
    }
}
