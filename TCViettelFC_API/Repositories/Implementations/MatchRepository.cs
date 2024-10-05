using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly ICloudinarySetting cloudinary;
        public MatchRepository(Sep490G53Context context, IConfiguration configuration , ICloudinarySetting _cloudinarySetting)
        {
            _context = context;
            _configuration = configuration;
            cloudinary  = _cloudinarySetting;
        }
        public async Task AddMatchesAsync( MatchesAddDto matchDto)
        {


            Match Matches = new Match
            {
                OpponentName = matchDto.OpponentName,
                StadiumName = matchDto.StadiumName,
                Status = matchDto.Status,
               
                IsHome = matchDto.IsHome,
                MatchDate = matchDto.MatchDate,
            };
            if (matchDto.LogoUrl != null && matchDto.LogoUrl.Length > 0 )
            {
                ImageUploadResult res = cloudinary.CloudinaryUpload(matchDto.LogoUrl);
                Matches.LogoUrl = res.SecureUrl.ToString();
            }
            else
            {
                Matches.LogoUrl = "/image/imagelogo/icon-image-not-found-free-vector.jpg";
            }
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
            if (match == null|| match.Status == 0) throw new KeyNotFoundException("Match not found");

            try
            {
                match.Status = 0;
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
            matches = await _context.Matches.Where(x => x.Id != 0).ToListAsync();
            return matches;
        }
        public async Task<Match> GetMatchesByIdAsync(int id)
        {
            Match matches = new Match();
            matches =  _context.Matches.FirstOrDefault(x => x.Id == id );   
            return matches;
        }


        public async Task UpdateMatchesAsync(int id, MatchesAddDto matchDto)
        {
            var matches = await _context.Matches.FindAsync(id);
            if (matches == null || matches.Status == 0)
            {
                throw new Exception("Matches not found");
            }

            // Update match properties
            matches.OpponentName = matchDto.OpponentName ?? matches.OpponentName;
            matches.StadiumName = matchDto.StadiumName ?? matches.StadiumName;
            matches.MatchDate = matchDto.MatchDate ?? matches.MatchDate;


            if (matchDto.LogoUrl != null && matchDto.LogoUrl.Length > 0)
            {
                ImageUploadResult res = cloudinary.CloudinaryUpload(matchDto.LogoUrl);
                matches.LogoUrl = res.SecureUrl.ToString();
            }
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
