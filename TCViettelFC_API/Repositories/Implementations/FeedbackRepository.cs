using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly Sep490G53Context _context;
        public FeedbackRepository(Sep490G53Context context)
        {
            _context = context;
         
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            return await _context.Feedbacks
             .Include(u => u.Creator)
             .ToListAsync();
        }

        public async Task<bool> UpdateFeedbackAsync(int feedbackId, int? responderId, int? status)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);

            if (feedback == null)
            {
                return false; // Feedback not found
            }

            feedback.ResponderId = responderId;
            feedback.Status = status;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true; // Update successful
        }
    }
}
