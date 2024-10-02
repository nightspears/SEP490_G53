﻿using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IMatchRepository
    {
   
        Task<List<Match>> GetMatchesAsync();
        Task AddMatchesAsync(MatchesAddDto matchDto);

        Task UpdateMatchesAsync(int id, MatchesAddDto matchDto);
        Task DeleteMatchesAsync(int id);
  

    }
}