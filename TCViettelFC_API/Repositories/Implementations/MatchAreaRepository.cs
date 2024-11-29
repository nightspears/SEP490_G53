using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCViettelFC_API.Dtos.MatchAreaTicket;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class MatchAreaRepository : IMatchAreaRepository
    {
        private readonly Sep490G53Context _context;

        public MatchAreaRepository(Sep490G53Context context)
        {
            _context = context;
     
        }
        public async Task<List<MatchAreaDto>> GetMatchAreaAsync()
        {
            var matchArea = await (from mat in _context.MatchAreaTickets
                                   join m in _context.Matches
                                   on mat.MatchId equals m.Id into matchGroup
                                   from m in matchGroup.DefaultIfEmpty() 
                                   group new { mat, m } by new { mat.MatchId, m.MatchDate,m.OpponentName,m.StadiumName } into grouped
                                   select new MatchAreaDto
                                   {
                                       Id = grouped.Key.MatchId,
                                       MatchDate = grouped.Key.MatchDate,
                                       OpponentName = grouped.FirstOrDefault().m.OpponentName,
                                       StadiumName = grouped.FirstOrDefault().m.StadiumName
                                   }).ToListAsync();

            return matchArea;
        }

        public async Task<List<MatchAreaRespone>> GetSanPhamById(int id)
        {
            var matchArea = _context.MatchAreaTickets.Where(x => x.MatchId == id).Include(a => a.Area).Include(p => p.Match).Select(m => new MatchAreaRespone
            {
                AreaId = m.AreaId,
                MatchId = m.MatchId,
                OpponentName = m.Match.OpponentName,
                StadiumName = m.Match.StadiumName,
                Stands = m.Area.Stands,
                Floor = m.Area.Floor,
                Section = m.Area.Section,
                AvailableSeats = m.AvailableSeats,
                Price = m.Area.Price,
                MatchDate = m.Match.MatchDate,
            }).ToList();
            return matchArea;
        }
        public void UpdateSeat(MatchAreaRequest matchArea)
        {
           var matchAreaticke = _context.MatchAreaTickets.FirstOrDefault(x => x.AreaId == matchArea.AreaId && x.MatchId == matchArea.MatchId);
            if (matchAreaticke != null)
            {
                if(matchAreaticke.AvailableSeats != matchArea.SeatQuantity)
                {
                    matchAreaticke.AvailableSeats = matchArea.SeatQuantity;
                    _context.SaveChanges();
                }
            }
          
        }

    }
}
