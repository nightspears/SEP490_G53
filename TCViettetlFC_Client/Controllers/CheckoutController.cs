using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using TCViettetlFC_Client.Models; // Assuming you create a CheckoutModel class here

namespace TCViettetlFC_Client.Controllers
{
    public class CheckoutController : Controller
    {
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
                // You can process the model data here, e.g., save to the database

                // Create a new ViewModel for the order summary
                var orderSummary = new CheckoutModel
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Address = model.Address,
                    City = model.City,
                    District = model.District,
                    Notes = model.Notes, 
                    SelectedShipping = model.SelectedShipping
                };

                // Redirect to the Summary action with the order summary
                return RedirectToAction("Summary", orderSummary);
            }
            // If the model state is not valid, return to the Index view
            return View("Index", model);
        }

        public IActionResult Summary(CheckoutModel model)
        {
            return View(model);
        }

    }

}
