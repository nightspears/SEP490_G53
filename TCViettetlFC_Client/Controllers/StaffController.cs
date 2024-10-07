using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TCViettetlFC_Client.Models;

namespace TCViettetlFC_Client.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;
        public StaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public IActionResult Home()
        {
            return View();
        }
        public async Task<IActionResult> StaffManagermentNew()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }

            var creatorId = Request.Cookies["UserId"]; 
            var status = 0; 
            var createdAt = DateTime.Now;
          
            string requestUri = "https://localhost:5000/api/New/GetAllNews";
            var response = await _httpClient.GetAsync(requestUri);
            List<GetAllNewViewModel> newList = new List<GetAllNewViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                newList = JsonConvert.DeserializeObject<List<GetAllNewViewModel>>(jsonData);
            }

            string categoryRequestUri = "https://localhost:5000/api/NewsCategory/GetAllCategoryNews";
            var categoryResponse = await _httpClient.GetAsync(categoryRequestUri);
            List<CategoryNewViewModel> categoryList = new List<CategoryNewViewModel>();

            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryJsonData = await categoryResponse.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<CategoryNewViewModel>>(categoryJsonData);
            }

            ViewData["NewList"] = newList;
            ViewData["CategoryList"] = categoryList;
            ViewData["CreatorId"] = creatorId; 
            ViewData["Status"] = status; 
            ViewData["CreateAt"] = createdAt; 

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNew(AddNewViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var token = Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
                var creatorId = model.creatorId;
                var newsCategoryId = model.newsCategoryId;
                var createdAt = model.createdAt;
                var status = model.status;

                var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

                
                string requestUri = "https://localhost:5000/api/New/create";
                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Đã thêm tin tức thành công";
                    return RedirectToAction("StaffManagermentNew");
                    
                }
                else
                {
                    
                    TempData["Message"] = "Failed to create new news item.";
                }
            }


            return RedirectToAction("StaffManagermentNew");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNew(int NewId, UpdateNewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }

                // Serialize the model to JSON
                var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

                // Construct the request URI for updating news
                string requestUri = $"https://localhost:5000/api/New/update/{NewId}";
                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Cập nhật tin tức thành công";
                    return RedirectToAction("StaffManagermentNew");
                }
                else
                {
                    TempData["Message"] = "Failed to update the news item.";
                }
            }

            return RedirectToAction("StaffManagermentNew");
         }

        [HttpPost]
        public async Task<IActionResult> DeleteNew(int id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Call the API to delete the news item
            string requestUri = $"https://localhost:5000/api/New/delete/{id}";
            var response = await _httpClient.DeleteAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Tin tức đã được xóa thành công";
            }
            else
            {
                TempData["Message"] = "Failed to delete the news item.";
            }

            return RedirectToAction("StaffManagermentNew");
        }

    }
}
