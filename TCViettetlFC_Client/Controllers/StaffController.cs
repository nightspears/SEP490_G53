using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TCViettelFC_Client.ApiServices;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;


namespace TCViettetlFC_Client.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly FeedbackService _feedbackService;
        private readonly OrderService _orderService;
        private readonly IApiHelper _apiHelper;
        private readonly GoShipService _goShipService;
        public StaffController(IHttpClientFactory httpClientFactory, FeedbackService feedbackService, IApiHelper apiHelper, OrderService orderService, GoShipService goShipService)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _feedbackService = feedbackService;
            _apiHelper = apiHelper;
            _orderService = orderService;
            _goShipService = goShipService;
        }
        private async Task<List<TicketOrdersViewModel>> GetAllTicketOrders()

        {
            return await _apiHelper.GetApiResponseAsync<List<TicketOrdersViewModel>>("order/getticketorders");
        }
        private async Task<List<OrderedTicketDto>> GetOrderedTicket(int id)
        {
            return await _apiHelper.GetApiResponseAsync<List<OrderedTicketDto>>($"order/getorderedticket/{id}");
        }
        private async Task<List<OrderedSuppItemDto>> GetOrderedSupp(int id)
        {
            return await _apiHelper.GetApiResponseAsync<List<OrderedSuppItemDto>>($"order/getorderedsupp/{id}");
        }
        public async Task<IActionResult> TicketOrders()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.Orders = await GetAllTicketOrders();
            return View();
        }
        public IActionResult Home()
        {
            var token = Request.Cookies["AuthToken"]; // Change this to AuthToken
            var roleId = Request.Cookies["RoleId"]; // Keep this for role verification

            // Check if token is missing or if roleId doesn't match
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(roleId) || roleId != "1")
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        public async Task<IActionResult> TicketOrderDetail(int id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
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



        public async Task<IActionResult> FeedbackManagement()
        {
            var token = Request.Cookies["AuthToken"];
            var feedbacks = await _feedbackService.GetFeedbacksAsync(token);
            return View(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveFeedback(int feedbackId)
        {
            var token = Request.Cookies["AuthToken"];

            // Logic to get the responder ID
            int responderId = GetResponderId(); // Implement this method to retrieve the responder ID

            // Call the feedback service to approve the feedback
            var success = await _feedbackService.ApproveFeedbackAsync(feedbackId, responderId, token);

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

        private int GetResponderId()
        {
            // Implement logic to retrieve the responder ID (e.g., from User.Identity)
            return 1; // Temporary hardcoded value for demonstration
        }


        public async Task<IActionResult> OrderProductManagement()
        {
            IEnumerable<OrderProductDto> orders;
            try
            {
                orders = await _orderService.GetAllOrderProductsAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to retrieve orders.";
                return RedirectToAction("ErrorPage");
            }
            return View(orders);
        }


        public async Task<IActionResult> OrderProductDetail(int id)
        {


           
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


    }
}
