using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCViettelFC_API.Dtos.Season;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class SeasonRepository : ISeasonRepository
    {
        private readonly Sep490G53Context _context;

        public SeasonRepository(Sep490G53Context context)
        {
            _context = context;

        }
        public async Task AddSeasonAsync(SeasonDto seasonDto)
        {

            Season season = new Season();
            {
                season.SeasonName = seasonDto.SeasonName;
                season.StartYear = seasonDto.StartYear;
                season.EndYear = seasonDto.EndYear;
                season.Status = seasonDto.Status;   
            };
          
            try
            {
                await _context.Seasons.AddAsync(season);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Season not found");
            }
        }
        public async Task DeleteSeasonAsync(int id)
        {
            var season = await _context.Seasons.FindAsync(id);
            if (season == null|| season.Status == 0) throw new KeyNotFoundException("Season not found");

            try
            {
                season.Status = 0;
                await _context.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Delete failed", ex);
            }
        }
      

        public async Task<List<SeasonResponse>> GetSeasonAsync()
        {
            List<SeasonResponse> seasons = new List<SeasonResponse>();
            seasons = await _context.Seasons.Where(x => x.Status != 0).Select(x => new SeasonResponse
            {
                SeasonId = x.SeasonId,
                EndYear = x.EndYear,
                Status = x.Status,  
                SeasonName = x.SeasonName,
                StartYear = x.StartYear,
            }).ToListAsync();

            return seasons;
        }
        public async Task<Season> GetSeasonByIdAsync(int id)
        {
            Season seasons = new Season();
            seasons = await _context.Seasons.FirstOrDefaultAsync(x => x.SeasonId == id && x.Status == 1);
              
            if (seasons == null)
            {
                throw new Exception("Season not found");
            }
            else
            {
                return seasons;
            }
        }
        public async Task UpdateSeasonAsync(int id, SeasonDto seasonDto)
        {
            try
            {
                var season = await _context.Seasons.FindAsync(id);
                if (season == null || season.Status == 0)
                {
                    throw new Exception("Season not found");
                }

                // Update Season properties
                season.SeasonName = seasonDto.SeasonName ?? season.SeasonName;
                season.Status = seasonDto.Status ?? season.Status;
                season.StartYear = seasonDto.StartYear ?? season.StartYear;
                season.EndYear = seasonDto.EndYear ?? season.EndYear;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Season: " + ex.Message);
            }
        }
    }
}
