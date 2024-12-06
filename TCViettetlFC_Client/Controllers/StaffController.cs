﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;


namespace TCViettetlFC_Client.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;
        private readonly FeedbackService _feedbackService;
        private readonly OrderService _orderService;
        private readonly IApiHelper _apiHelper;
        private readonly GoShipService _goShipService;
        private readonly int itemPerPage = 14;
        public StaffController(IHttpClientFactory httpClientFactory, FeedbackService feedbackService, IApiHelper apiHelper, OrderService orderService, GoShipService goShipService, IWebHostEnvironment env)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _feedbackService = feedbackService;
            _apiHelper = apiHelper;
            _orderService = orderService;
            _goShipService = goShipService;
            _env = env;
        }
        private async Task<List<TicketOrdersViewModel>> GetAllTicketOrders(int page)
        {
            return await _apiHelper.GetApiResponseAsync<List<TicketOrdersViewModel>>($"order/getticketorders?$skip={(page - 1) * itemPerPage}&$top={itemPerPage}");
        }
        private async Task<List<OrderedTicketDto>> GetOrderedTicket(int id)
        {
            return await _apiHelper.GetApiResponseAsync<List<OrderedTicketDto>>($"order/getorderedticket/{id}");
        }
        private async Task<List<OrderedSuppItemDto>> GetOrderedSupp(int id)
        {
            return await _apiHelper.GetApiResponseAsync<List<OrderedSuppItemDto>>($"order/getorderedsupp/{id}");
        }
        public async Task<IActionResult> TicketOrders(int id = 1)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }
            var result = await _apiHelper.GetApiResponseAsync<List<TicketOrdersViewModel>>("order/getticketorders");
            int totalPage = (int)Math.Ceiling((double)result.Count() / itemPerPage);
            if (id > totalPage) id = totalPage;
            if (id < 1) id = 1;
            TempData["TotalPages"] = totalPage;
            var orders = await GetAllTicketOrders(id);
            return View(orders);
        }


        public async Task<IActionResult> TicketOrderDetail(int id)
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            var sup = await GetOrderedSupp(id);
            var ticket = await GetOrderedTicket(id);
            var viewModel = new TicketOrderDetailModel
            {
                Sup = sup,
                Ticket = ticket
            };
            return View(viewModel);
        }

        public async Task<IActionResult> StaffManagermentNew()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
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

                // Chuẩn bị nội dung gửi đi dưới dạng MultipartFormDataContent
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(model.creatorId.ToString()), "CreatorId");
                content.Add(new StringContent(model.newsCategoryId.ToString()), "NewsCategoryId");
                content.Add(new StringContent(model.title ?? ""), "Title");
                content.Add(new StringContent(model.content ?? ""), "Content");
                content.Add(new StringContent(model.createdAt.HasValue ? model.createdAt.Value.ToString("o") : ""), "CreatedAt"); // ISO 8601 format
                content.Add(new StringContent(model.status.ToString()), "Status");

                // Nếu có tệp hình ảnh, thêm vào nội dung
                if (model.image != null && model.image.Length > 0)
                {
                    var fileContent = new StreamContent(model.image.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.image.ContentType);
                    content.Add(fileContent, "Image", model.image.FileName);
                }

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
        public async Task<IActionResult> UpdateNew(int NewId, UpdateNewViewModel model, string? currentImage)
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

                using var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent(model.creatorId.ToString()), "CreatorId");
                multipartContent.Add(new StringContent(model.newsCategoryId.ToString()), "NewsCategoryId");
                multipartContent.Add(new StringContent(model.title), "Title");
                multipartContent.Add(new StringContent(model.content), "Content");
                multipartContent.Add(new StringContent(model.createdAt.HasValue ? model.createdAt.Value.ToString("o") : ""), "CreatedAt");

                if (model.image != null && model.image.Length > 0)
                {
                    // Nếu có ảnh mới, sử dụng ảnh này
                    var streamContent = new StreamContent(model.image.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.image.ContentType);
                    multipartContent.Add(streamContent, "Image", model.image.FileName);
                }
                else if (!string.IsNullOrEmpty(currentImage))
                {
                    // Nếu không có ảnh mới, sử dụng ảnh cũ
                    multipartContent.Add(new StringContent(currentImage), "ImagePath");
                }

                string requestUri = $"https://localhost:5000/api/New/update/{NewId}";
                var response = await _httpClient.PostAsync(requestUri, multipartContent);

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



        public async Task<IActionResult> FeedbackManagement()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            var token = Request.Cookies["AuthToken"];
            var feedbacks = await _feedbackService.GetFeedbacksAsync(token);
            return View(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveFeedback(int feedbackId)
        {
            var token = Request.Cookies["AuthToken"];

            var responderId = GetResponderId();

            if (responderId == null)
            {
                TempData["ErrorMessage"] = "Unable to approve feedback. Responder ID not found.";
                return RedirectToAction("FeedbackManagement");
            }


            // Call the feedback service to approve the feedback
            var success = await _feedbackService.ApproveFeedbackAsync(feedbackId, responderId.Value, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Feedback approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve feedback.";
            }

            // Redirect back to FeedbackManagement view
            return RedirectToAction("FeedbackManagement");
        }
        private int? GetResponderId()
        {
            if (Request.Cookies.TryGetValue("UserId", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            // Return null if the cookie does not exist or cannot be parsed as an integer
            return null;
        }


        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> OrderProductManagement()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            try
            {
                var orders = await _orderService.GetAllOrderProductsAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to retrieve orders.";
                return RedirectToAction("ErrorPage");
            }
        }
        public IActionResult SeatManagement()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        public IActionResult SupplementaryItem()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }

        public IActionResult ProductManagement()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        public IActionResult MatchManagement()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        public IActionResult CategoryProduct()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        public IActionResult Season()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        public IActionResult Discount()
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            return View();
        }
        public async Task<IActionResult> SeatEdit(int id)
        {
            ViewBag.Id = id;

            string requestUri = "https://localhost:5000/api/MatchAreas/GetSanPhamById?id=" + id;
            var response = await _httpClient.GetAsync(requestUri);

            List<MatchArea> ListData = new List<MatchArea>();

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                ListData = JsonConvert.DeserializeObject<List<MatchArea>>(jsonData);
            }
            ViewBag.DoiThu = ListData.FirstOrDefault().OpponentName;
            ViewBag.TenSan = ListData.FirstOrDefault().StadiumName;
            ViewBag.NgayDa = ListData.FirstOrDefault().MatchDate;
            return View(ListData);
        }

        public async Task<IActionResult> OrderProductDetail(int id)
        {


            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            string UserId = Request.Cookies["UserId"];
            ViewData["UserId"] = UserId;
            var orderDetail = await _orderService.GetOrderDetailsAsync(id /*, token*/);

            if (orderDetail == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("OrderProductManagement");
            }

            return View(orderDetail); // Pass OrderDetailDto to the view
        }

        public async Task<IActionResult> OrderShipmentDetail(string trackingCode)
        {
            var cookies = Request.Cookies["RoleId"];
            if (cookies != "1")
            {
                return RedirectToAction("Index", "Forbidden");
            }
            if (string.IsNullOrEmpty(trackingCode))
            {
                return BadRequest("Tracking code is required.");
            }
            var shipmentResponse = await _goShipService.GetShipmentAsync(trackingCode);

            if (shipmentResponse == null || shipmentResponse.data == null || shipmentResponse.data.Count == 0)
            {
                return NotFound("Shipment not found.");
            }

            // Assuming you want to display the first shipment data
            var shipmentData = shipmentResponse.data.FirstOrDefault();

            return View(shipmentData);
        }
        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> files)
        {
            var filepath = "";
            foreach (IFormFile photo in Request.Form.Files)
            {
                string serverMapPath = Path.Combine(_env.WebRootPath, "Image", photo.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                filepath = "https://tcviettelfc.azurewebsites.net/" + "Image/" + photo.FileName;
            }

            return Json(new { url = filepath }); ;
        }



    }
}
