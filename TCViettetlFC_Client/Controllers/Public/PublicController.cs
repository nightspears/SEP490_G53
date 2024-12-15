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
        [Route("public/sanpham")]
        public IActionResult Product()
        {
            return View();
        }
        [Route("public/ChiTietSanPham/{id}")]
        public async Task<IActionResult> DetailProduct(int id)
        {
           

            string requestUri = "https://tcvtfcapi.azurewebsites.net/api/Product/GetSanPhamById?id=" + id;
            var response = await _httpClient.GetAsync(requestUri);

           ApiResponse ListData = new ApiResponse();

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Id = id;
                var jsonData = await response.Content.ReadAsStringAsync();
                ListData = JsonConvert.DeserializeObject<ApiResponse>(jsonData);
                return View(ListData);
            }
            else
            {
                return RedirectToAction("404", "Error");
            }
          
        }

        [Route("public/giohang")]
        public IActionResult Cart()
        {
            return View();
        }
        
    }
}
