﻿using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IMatchRepository
    {
   
        Task<List<Match>> GetMatchesAsync();
        Task AddMatchesAsync(MatchesAddDto matchDto);
        Task<Match> GetMatchesByIdAsync(int id);
        Task UpdateMatchesAsync(int id, MatchesAddDto matchDto);
        Task DeleteMatchesAsync(int id);
        Task<List<Match>> GetMatchesNotStartYetAsync();
        void UpdateStatus(int status,int id);
        JsonResult CheckExist(CheckMatch data);
    }
}
