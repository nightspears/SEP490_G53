using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Text.Json.Nodes;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;
using TCViettetlFC_Client.VNPayHelper; // Assuming you create a CheckoutModel class here

    namespace TCViettetlFC_Client.Controllers
    {
        public class CheckoutController : Controller
        {
            private readonly IVnPayService _vnpayService;
        private readonly CheckOutService _checkOutService; // Add CheckOutService

        public CheckoutController(IVnPayService vnpayService, CheckOutService checkOutService)
        {
            _vnpayService = vnpayService;
            _checkOutService = checkOutService; // Initialize CheckOutService
        }

        public IActionResult Index(string data)
           {
            SetCustomerInfoInViewData();

            var checkoutModel = new CheckoutModel();
                checkoutModel.checkoutItems = new List<CheckoutCartModel>();
                decimal totalPrice = 0;

                if (!string.IsNullOrEmpty(data))
                {
                    // Deserialize the JSON data to a list of CheckoutCartModel
                    checkoutModel.checkoutItems = JsonConvert.DeserializeObject<List<CheckoutCartModel>>(data);

                    // Calculate the total price based on the deserialized items
                    // Calculate the total price and convert it to an integer
                    checkoutModel.TotalAmount = (int)checkoutModel.checkoutItems.Sum(item => item.Price * item.Quantity);

                }

          

                // Pass the model to the checkout view
                return View(checkoutModel);
            }
        public void SetCustomerInfoInViewData()
        {
            string customerId = Request.Cookies["CustomerId"];
            string customerPhone = Request.Cookies["CustomerPhone"];
            string customerEmail = Request.Cookies["CustomerEmail"];

            // Check if the cookies exist and pass them to ViewData
            ViewData["CustomerId"] = customerId ?? string.Empty;
            ViewData["CustomerPhone"] = customerPhone ?? string.Empty;
            ViewData["CustomerEmail"] = customerEmail ?? string.Empty;
        }

        [HttpPost]
            public IActionResult SubmitCheckout(CheckoutModel model)
            {
                if (ModelState.IsValid)
                {
                    // Save the CheckoutModel to TempData for later retrieval.
                    TempData["CheckoutModel"] = JsonConvert.SerializeObject(model);

                    var vnPayModel = new VNPaymentRequestModel
                    {
                        Amount = model.TotalAmount,
                        CreatedDate = DateTime.Now,
                        Decription = "Payment for order test",
                        FullName = model.FullName, // Replace with actual customer data
                        OrderId = 1 // You may need to generate the OrderId dynamically
                    };

                    // Redirect to VnPay payment page
                    var paymentUrl = _vnpayService.CreatePaymentUrl(HttpContext, vnPayModel,false);
                    if (!string.IsNullOrEmpty(paymentUrl))
                    {
                        return Redirect(paymentUrl);
                    }
                }

                // If the model state is not valid, return to the Index view with the model.
                return View("Index", model);
            }



        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnpayService.PaymentExecute(Request.Query);

            // Retrieve the CheckoutModel from TempData
            CheckoutModel checkoutModel = null;
            if (TempData["CheckoutModel"] != null)
            {
                checkoutModel = JsonConvert.DeserializeObject<CheckoutModel>(TempData["CheckoutModel"].ToString());
                TempData.Keep("CheckoutModel"); // Keep TempData for future use if needed.
            }

            if (response != null)
            {
                if (response.Success && response.VnPayResponseCode == "00")
                {
                    try
                    {
                        // Prepare the CreateOrderRequest based on checkoutModel
                        var createOrderRequest = new CreateOrderRequest
                        {
                            Customer = new CustomerDTO
                            {
                                AccountId=checkoutModel.AccountId,
                                Email = checkoutModel.Email, // Add actual email field if exists
                                Phone = checkoutModel.Phone,// Add actual phone field if exists
                                FullName = checkoutModel.FullName
                            },
                            Address = new AddressDTO
                            {
                                City = checkoutModel.CityId,
                                CityName = checkoutModel.CityName,
                                District = checkoutModel.DistrictId,
                                DistrictName = checkoutModel.DistrictName,
                                Ward = checkoutModel.WardId,
                                WardName = checkoutModel.WardName,
                                DetailedAddress = checkoutModel.Address
                            },
                            OrderProduct = new OrderProductDTO
                            {
                                OrderCode = checkoutModel.SelectedShipping, // Use the actual order code
                                OrderDate = DateTime.Now,
                                ShipmentFee = checkoutModel.ShipmentFee,
                                TotalPrice = checkoutModel.TotalAmount
                            },
                            OrderProductDetails = checkoutModel.checkoutItems.Select(item => new OrderProductDetailDTO
                            {
                                ProductId = item.ProductId,
                                PlayerId = item.shirtNumber <= 0 ? null : item.shirtNumber,
                                CustomShirtNumber = item.SoAo,
                                CustomShirtName = item.TenCauThu,
                                Size = item.size,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                ProductName = item.nameProduct,
                                Avatar = item.Avartar
                            }).ToList(),
                            Payment = new PaymentDTO
                            {
                                TotalAmount = checkoutModel.TotalAmount,
                                PaymentGateway = response.TransactionId // Assuming it's VNPay
                            }
                        };

                        // Call the CreateOrderAsync method to insert data into the database
                        var result = await _checkOutService.CreateOrderAsync(createOrderRequest);

                        // Handle the result (e.g., log it, show success message, etc.)
                        // Redirect or display a view accordingly
                        return View("PaymentCallBack", model: result);
                       

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        throw;
                    }
                }
                else if (response.VnPayResponseCode == "24") // VnPay response code for "Transaction cancelled by user"
                {
                    // Return to the checkout page if the user cancels the payment
                    SetCustomerInfoInViewData();
                    return View("Index", checkoutModel); // Pass the retrieved CheckoutModel back to the checkout page
                }
                else
                {
                    // Handle other failure scenarios
                    return View("PaymentCallBack", model: "Error");
                }
            }

            // In case the response is null or something went wrong, handle it gracefully
            return View("PaymentCallBack", model: "Error");
        }
    }
    }
