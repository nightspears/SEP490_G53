using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.VNPayHelper; // Assuming you create a CheckoutModel class here

namespace TCViettetlFC_Client.Controllers
{
    namespace TCViettetlFC_Client.Controllers
    {
        public class CheckoutController : Controller
        {
            private readonly IVnPayService _vnpayService;

            public CheckoutController(IVnPayService vnpayService)
            {
                _vnpayService = vnpayService;
            }

            // GET: Checkout
            public IActionResult Index()
            {
                return View(new CheckoutModel());
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

            public IActionResult Summary(CheckoutModel model)
            {
                return View(model);
            }

            public IActionResult PaymentCallBack()
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
                        // Payment was successful, pass the checkout model and response to the view
                        return View("PaymentCallBack", new PaymentSuccessViewModel
                        {
                            CheckoutModel = checkoutModel,
                            VnPayResponse = response
                        });
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
}