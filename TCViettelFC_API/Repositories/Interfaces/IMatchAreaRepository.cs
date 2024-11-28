using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using TCViettelFC_API.Dtos.MatchAreaTicket;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IMatchAreaRepository
    {
   
        Task<List<MatchAreaDto>> GetMatchAreaAsync();
        Task<List<MatchAreaRespone>> GetSanPhamById(int id);
        void UpdateSeat(MatchAreaRequest matchArea);

        
    }
}
