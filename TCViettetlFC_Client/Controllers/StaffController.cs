using Microsoft.AspNetCore.Mvc;
using TCViettelFC_Client.Services;
using TCViettetlFC_Client.Models;
using TCViettetlFC_Client.Services;

namespace TCViettetlFC_Client.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FeedbackService _feedbackService;

        public StaffController(IHttpClientFactory httpClientFactory, FeedbackService feedbackService)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _feedbackService = feedbackService;
        }

        public IActionResult Home()
        {
            return View();
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
    }
}
