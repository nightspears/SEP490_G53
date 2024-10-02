using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class MatchRepository : IMatchRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        public MatchRepository(Sep490G53Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task AddMatchesAsync( MatchesAddDto matchDto)
        {
            var Matches = new Match
            {
                OpponentName = matchDto.OpponentName,
                StadiumName = matchDto.StadiumName,
                Status = matchDto.Status,
                LogoUrl = matchDto.LogoUrl,
                IsHome = matchDto.IsHome,
                MatchDate = matchDto.MatchDate,
            };
            try
            {
                await _context.Matches.AddAsync(Matches);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task DeleteMatchesAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) throw new KeyNotFoundException("Match not found");

            try
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Delete failed", ex);
            }
        }


        public async Task<List<Match>> GetMatchesAsync()
        {
            List<Match> matches = new List<Match>();
            matches = await _context.Matches.ToListAsync();
            return matches;
        }


        public async Task UpdateMatchesAsync(int id, MatchesAddDto matchDto)
        {
            var matches = await _context.Matches.FindAsync(id);
            if (matches == null)
            {
                throw new Exception("User not found");
            }

            // Update match properties
            matches.OpponentName = matchDto.OpponentName ?? matches.OpponentName;
            matches.StadiumName = matchDto.StadiumName ?? matches.StadiumName;
            matches.MatchDate = matchDto.MatchDate ?? matches.MatchDate;
            matches.LogoUrl = matchDto.LogoUrl ?? matches.LogoUrl;
            matches.Status = matchDto.Status ?? matches.Status;
            matches.IsHome = matchDto.IsHome ?? matches.IsHome; 
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating match: " + ex.Message);
            }
        }
    }
}
