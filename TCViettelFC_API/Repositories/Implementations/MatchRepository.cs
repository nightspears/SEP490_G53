﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
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
        
        private readonly ICloudinarySetting cloudinary;
        public MatchRepository(Sep490G53Context context, ICloudinarySetting _cloudinarySetting)
        {
            _context = context;
            cloudinary = _cloudinarySetting;
        }
        public async Task AddMatchesAsync(MatchesAddDto matchDto)
        {

            if (matchDto.MatchDate < DateTime.Now)
            {
                throw new ArgumentException("match date must greater than today");
            }
           
            if (string.IsNullOrEmpty(matchDto.OpponentName))
            {
                throw new ArgumentException("The system returns an error, no new match are added.");
            }
           
            if (string.IsNullOrEmpty(matchDto.MatchDate.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new match are added.");
            }
            if (string.IsNullOrEmpty(matchDto.StadiumName))
            {
                throw new ArgumentException("The system returns an error, no new match are added.");
            }
            if (string.IsNullOrEmpty(matchDto.IsHome.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new match are added.");
            }


            try
            {
                Match Matches = new Match
                {
                    OpponentName = matchDto.OpponentName,
                    StadiumName = matchDto.StadiumName,
                    Status = matchDto.Status == null ? 2 : matchDto.Status,

                    IsHome = matchDto.IsHome,
                    MatchDate = matchDto.MatchDate,
                };
                if (matchDto.LogoUrl != null && matchDto.LogoUrl.Length > 0)
                {
                    ImageUploadResult res = cloudinary.CloudinaryUpload(matchDto.LogoUrl);
                    Matches.LogoUrl = res.SecureUrl.ToString();
                }
                else
                {
                    Matches.LogoUrl = "/image/imagelogo/ImageFail.jpg";
                }


                await _context.Matches.AddAsync(Matches);
                await _context.SaveChangesAsync();

                if(Matches.IsHome == true)
                {
                    var lstArea = _context.Areas.ToList();
                    var matchAreaTickets = new List<MatchAreaTicket>();

                    foreach (var area in lstArea)
                    {
                        matchAreaTickets.Add(new MatchAreaTicket
                        {
                            MatchId = Matches.Id,
                            AreaId = area.Id,
                            AvailableSeats = 100
                        });
                    }

                    await _context.MatchAreaTickets.AddRangeAsync(matchAreaTickets);
                    await _context.SaveChangesAsync();
                }
               
            }
            catch (Exception ex)
            {
            }
        }

        public async Task DeleteMatchesAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null || match.Status == 0) throw new KeyNotFoundException("Match not found");

            try
            {
                match.Status = 0;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Delete failed", ex);
            }
        }

        public async Task<List<Match>> GetMatchesAsync()
        {
            List<Match> matches = new List<Match>();
            matches = await _context.Matches.Where(x => x.Status != 0).ToListAsync();
            return matches;
        }

        public async Task<List<Match>> GetMatchesNotStartYetAsync()
        {
            List<Match> matches = new List<Match>();
            DateTime currentDate = DateTime.Now;
            DateTime threeDaysLater = currentDate.AddDays(8); 

            matches = await _context.Matches
                                    .Where(x => x.Status == 1 && x.MatchDate > currentDate && x.MatchDate <= threeDaysLater && x.IsHome == true)
                                    .ToListAsync();

            return matches;
        }

        public async Task<Match> GetMatchesByIdAsync(int id)
        {
            Match matches = new Match();
            matches = _context.Matches.FirstOrDefault(x => x.Id == id);
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

        public void UpdateStatus(int status, int id)
        {
            var match = _context.Matches.Find(id);
            match.Status = status;
            _context.SaveChanges();
        }

        public JsonResult CheckExist(CheckMatch checkMatch)
        {
            var mess = "";
            var exists = false;
            var type = 0;
            DateTime ngayDa;
            if (!DateTime.TryParse(checkMatch.NgayDa, out ngayDa))
            {
                exists = true;
                mess = "Ngày đá không hợp lệ.";
                type = 3;
            }

            var match = _context.Matches.FirstOrDefault(x => x.StadiumName.Equals(checkMatch.TenSan) && x.OpponentName.Equals(checkMatch.TenDoiThu) && (x.MatchDate == ngayDa) && x.Status != 0);
            if (match != null)
            {
                exists = true;
                mess = "Trận đấu này đã tồn tại";
                type = 1;
            }
            else
            {
                var startTime = ngayDa.AddHours(-24);
                var endTime = ngayDa.AddHours(24);

                var matchcheck = _context.Matches.FirstOrDefault(
                    x => x.MatchDate.HasValue
                      && x.MatchDate >= startTime
                      && x.MatchDate <= endTime && x.Status != 0
                );

                if (matchcheck != null)
                {
                    exists = true;
                    mess = "Khung giờ này đã có trận đấu khác";
                    type = 2;
                }
            }

            var data = new
            {
                mess = mess,
                exists = exists,
                Type = type,

            };

            return new JsonResult(data);

        }

    }
}
