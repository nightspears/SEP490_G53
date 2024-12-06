using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICustomerRepository _cR;
        private readonly IMapper _mapper;
        public FeedbackController(IFeedbackRepository feedbackRepository, IMapper mapper, ICustomerRepository cR)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
            _cR = cR;
        }

        [HttpPost("addfeedback")]
        public async Task<IActionResult> AddFeedback(FeedbackPostDto feedbackDto)
        {
            var result = await _cR.PostFeedback(feedbackDto);
            if (result == 0) return Conflict("Error adding feedback");
            return Ok("Feedback added successfully");
        }

        // Get a list of users
        [HttpGet("GetFeedbacks")]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedbacks()
        {
            var feedbacks = await _feedbackRepository.GetFeedbackAsync();
            List<FeedbackDto> results = new List<FeedbackDto>();
            foreach (var feedback in feedbacks)
            {
                results.Add(new FeedbackDto()
                {
                    Id = feedback.Id,
                    Content = feedback.Content,
                    CreatedAt = feedback.CreatedAt,
                    Email = feedback.Creator.Account.Email,
                    Phone = feedback.Creator.Account.Phone,
                });
            }
            return Ok(results);
        }

        // Update feedback
        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] UpdateFeedbackDto updateFeedbackDto)
        {
            if (updateFeedbackDto == null)
            {
                return BadRequest("Invalid feedback data.");
            }

            var result = await _feedbackRepository.UpdateFeedbackAsync(feedbackId, updateFeedbackDto.ResponderId, updateFeedbackDto.Status);

            if (result)
            {
                return Ok("Feedback updated successfully.");
            }
            else
            {
                return NotFound("Feedback not found.");
            }
        }
    }
}
