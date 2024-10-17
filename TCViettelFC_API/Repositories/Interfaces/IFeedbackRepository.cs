using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {


        Task<IEnumerable<Feedback>> GetFeedbackAsync();

        Task<bool> UpdateFeedbackAsync(int feedbackId, int? responderId, int? status);

    }

  
}
