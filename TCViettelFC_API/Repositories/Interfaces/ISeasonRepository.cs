using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Dtos.Season;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ISeasonRepository
    {
        Task<List<SeasonResponse>> GetSeasonAsync();

        Task AddSeasonAsync(SeasonDto season);
        Task<Season> GetSeasonByIdAsync(int id);
        Task UpdateSeasonAsync(int id, SeasonDto season);
        Task DeleteSeasonAsync(int id);

    }
}
