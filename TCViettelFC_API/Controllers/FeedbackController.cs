using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        public FeedbackController(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }


        // Get a list of users
        [HttpGet("GetFeedbacks")]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedbacks()
        {
            var feedbacks = await _feedbackRepository.GetFeedbackAsync();
            var feedbackDtos = _mapper.Map<IEnumerable<FeedbackDto>>(feedbacks);
            return Ok(feedbackDtos);
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
