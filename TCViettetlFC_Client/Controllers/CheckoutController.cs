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
                        OrderId = 222 // You may need to generate the OrderId dynamically
                    };

                    // Redirect to VnPay payment page
                    var paymentUrl = _vnpayService.CreatePaymentUrl(HttpContext, vnPayModel);
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
                                Email = checkoutModel.Email, // Add actual email field if exists
                                Phone = checkoutModel.Phone // Add actual phone field if exists
                            },
                            Address = new AddressDTO
                            {
                                City = checkoutModel.City,
                                District = checkoutModel.District,
                                Ward = checkoutModel.Ward,
                                DetailedAddress = checkoutModel.Address
                            },
                            OrderProduct = new OrderProductDTO
                            {
                                OrderCode = checkoutModel.SelectedShipping, // Use the actual order code
                                OrderDate = DateTime.Now,
                                TotalPrice = checkoutModel.TotalAmount
                            },
                            OrderProductDetails = checkoutModel.checkoutItems.Select(item => new OrderProductDetailDTO
                            {
                                ProductId = item.ProductId,
                                PlayerId = item.playerId == 0 ? null : item.playerId,
                                Size = item.size,
                                Quantity = item.Quantity,
                                Price = item.Price
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
                        return View("PaymentCallBack", new PaymentSuccessViewModel
                        {
                            CheckoutModel = checkoutModel,
                            VnPayResponse = response,
                        });

                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }
                else if (response.VnPayResponseCode == "24") // VnPay response code for "Transaction cancelled by user"
                {
                    // Return to the checkout page if the user cancels the payment
                    ViewBag.ErrorMessage = "Payment was cancelled. Please try again or choose a different payment method.";
                    return View("Index", checkoutModel); // Pass the retrieved CheckoutModel back to the checkout page
                }
                else
                {
                    // Handle other failure scenarios
                    ViewBag.ErrorMessage = "Payment failed. Please try again.";
                    return View("Index", checkoutModel);
                }
            }

            // In case the response is null or something went wrong, handle it gracefully
            ViewBag.ErrorMessage = "Invalid response from VnPay.";
            return View("Index", checkoutModel);
        }
    }
    }
