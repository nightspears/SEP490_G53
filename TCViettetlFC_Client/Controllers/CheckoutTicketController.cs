using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;
using TCViettetlFC_Client.VNPayHelper;

namespace TCViettetlFC_Client.Controllers
{
    public class CheckoutTicketController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IVnPayService _vnpayService;
        public CheckoutTicketController(IHttpClientFactory httpClientFactory, IVnPayService vnpayService)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _vnpayService = vnpayService;
        }
        public IActionResult Checkout()
        {
            string customerId = Request.Cookies["CustomerId"];
            ViewData["CustomerId"] = customerId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTicketOrder([FromBody] TicketOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            // Save request to TempData for future use if needed
            TempData["CheckoutTicketModel"] = JsonConvert.SerializeObject(request);

            try
            {
                var vnPayModel = new VNPaymentRequestModel
                {
                    Amount = (int)request.totalAmount,
                    CreatedDate = DateTime.Now,
                    Decription = "Payment for ticket order",
                    FullName = request.addCustomerDto?.fullName ?? "Login user",
                    OrderId = 2 // Assume this method generates a unique OrderId
                };

                // Generate VnPay payment URL
                var paymentUrl = _vnpayService.CreatePaymentUrl(HttpContext, vnPayModel, true);

                if (string.IsNullOrEmpty(paymentUrl))
                {
                    return BadRequest(new { success = false, message = "Failed to generate payment URL." });
                }

                // Return the payment URL in JSON so frontend can handle redirection
                return Ok(new { success = true, paymentUrl });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error during ticket order submission: " + ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
            }
        }


        public async Task<IActionResult> TicketPaymentCallback()
        {
            var VNPayresponse = _vnpayService.PaymentExecute(Request.Query);
            TicketOrderRequest request = null;

            if (TempData["CheckoutTicketModel"] != null)
            {
                request = JsonConvert.DeserializeObject<TicketOrderRequest>(TempData["CheckoutTicketModel"].ToString());
                TempData.Keep("CheckoutTicketModel");
            }

            var viewModel = new PaymentResultViewModel();

            try
            {
                // Check if the payment was successful (assuming "00" means success)
                if (VNPayresponse.VnPayResponseCode == "00")
                {
                    // Prepare URL for API call to insert order in the database
                    string url = request.customerId.HasValue
                        ? $"https://localhost:5000/api/TicketOrder?customerId={request.customerId}"
                        : "https://localhost:5000/api/TicketOrder";

                    // Update the request object with the payment transaction details
                    request.paymentDto.paymentGateway = VNPayresponse.TransactionId;
                    var payload = JsonConvert.SerializeObject(request);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");

                    // Make API call to save the ticket order in the database
                    var response = await _httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["clearCart"] = true;
                        viewModel.Message = "Đặt vé thành công!";
                        viewModel.IsSuccess = true;
                        return View(viewModel);
                    }
                    else
                    {
                        var errorData = await response.Content.ReadAsStringAsync();
                        Console.Error.WriteLine("API Error Details:", errorData);

                        viewModel.Message = "Đặt vé không thành công. Vui lòng thử lại.";
                        viewModel.IsSuccess = false;
                        return View(viewModel);
                    }
                }
                else if (VNPayresponse.VnPayResponseCode == "24") // Transaction cancelled by user
                {
                    viewModel.Message = "Payment was cancelled. Please try again or choose a different payment method.";
                    viewModel.IsSuccess = false;
                    return View("Checkout", viewModel);
                }
                else if (VNPayresponse.VnPayResponseCode == "51") // Insufficient funds
                {
                    viewModel.Message = "Insufficient funds in your account. Please try a different payment method.";
                    viewModel.IsSuccess = false;
                    return View("Checkout", viewModel);
                }
                else if (VNPayresponse.VnPayResponseCode == "99") // Payment timeout or system error
                {
                    viewModel.Message = "Payment timed out or encountered a system error. Please try again.";
                    viewModel.IsSuccess = false;
                    return View("Checkout", viewModel);
                }
                else
                {
                    // Handle other failure scenarios
                    viewModel.Message = "Payment failed. Please try again.";
                    viewModel.IsSuccess = false;
                    return View("Checkout", viewModel);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error:", ex);
                viewModel.Message = "Đã xảy ra lỗi trong quá trình đặt vé. Vui lòng thử lại.";
                viewModel.IsSuccess = false;
                return View(viewModel);
            }
        }

        public class PaymentResultViewModel
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
        }
    }
}
