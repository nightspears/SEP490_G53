using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly Sep490G53Context _context;

        public PlayerRepository(Sep490G53Context context)
        {
            _context = context;
        }

        public async Task<PlayerDto> AddPlayerAsync(PlayerDto playerDto)
        {
            try
            {
                var player = new Player
                {
                    FullName = playerDto.FullName,
                    ShirtNumber = playerDto.ShirtNumber,
                    SeasonId = playerDto.SeasonId,
                    Position = playerDto.Position,
                    JoinDate = playerDto.JoinDate,
                    OutDate = playerDto.OutDate,
                    Description = playerDto.Description,
                    BackShirtImage = playerDto.BackShirtImage,
                    Status = 1,
                };

                _context.Players.Add(player);
                await _context.SaveChangesAsync();

                playerDto.PlayerId = player.PlayerId;
                return playerDto;
            }
            catch (Exception ex)
            {
                throw new Exception("lỗi không thêm được cầu thủ", ex);
            }
        }

        public async Task<PlayerDto> DeletePlayerAsync(int id)
        {
            try
            {
                var player = await _context.Players.FindAsync(id);
                if (player == null || player.Status == 0)
                    throw new KeyNotFoundException("không thấy cầu thủ");

                player.Status = 0; // Soft delete
                await _context.SaveChangesAsync();

                return new PlayerDto
                {
                    PlayerId = player.PlayerId,
                    FullName = player.FullName,
                    ShirtNumber = player.ShirtNumber,
                    SeasonId = player.SeasonId,
                    Position = player.Position,
                    JoinDate = player.JoinDate,
                    OutDate = player.OutDate,
                    Description = player.Description,
                    BackShirtImage = player.BackShirtImage,
                    Status = player.Status
                };
            }
            catch (Exception ex)
            {
                throw new Exception("không xóa được cầu thủ", ex);
            }
        }

        public async Task<PlayerDto> GetPlayerByIdAsync(int id)
        {
            try
            {
                var player = await _context.Players
                    .Where(p => p.PlayerId == id)
                    .FirstOrDefaultAsync();

                if (player == null)
                    throw new KeyNotFoundException("không tìm được cầu thủ với id đó");

                return new PlayerDto
                {
                    PlayerId = player.PlayerId,
                    FullName = player.FullName,
                    ShirtNumber = player.ShirtNumber,
                    SeasonId = player.SeasonId,
                    Position = player.Position,
                    JoinDate = player.JoinDate,
                    OutDate = player.OutDate,
                    Description = player.Description,
                    BackShirtImage = player.BackShirtImage,
                    Status = player.Status
                };
            }
            catch (Exception ex)
            {
                throw new Exception("không lấy được cầu thủ", ex);
            }
        }

        public async Task<List<PlayerDto>> ListAllPlayerAsync()
        {
            try
            {
                return await _context.Players
                    .Select(player => new PlayerDto
                    {
                        PlayerId = player.PlayerId,
                        FullName = player.FullName,
                        ShirtNumber = player.ShirtNumber,
                        SeasonId = player.SeasonId,
                        Position = player.Position,
                        JoinDate = player.JoinDate,
                        OutDate = player.OutDate,
                        Description = player.Description,
                        BackShirtImage = player.BackShirtImage,
                        Status = player.Status
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("không liệt kê được cầu thủ", ex);
            }
        }

        public async Task<PlayerDto> UpdatePlayerAsync(PlayerDto playerDto)
        {
            try
            {
                var player = await _context.Players.FindAsync(playerDto.PlayerId);
                if (player == null)
                    throw new KeyNotFoundException("không tìm được cầu thủ để cập nhật");

                player.FullName = playerDto.FullName ?? player.FullName;
                player.ShirtNumber = playerDto.ShirtNumber ?? player.ShirtNumber;
                player.SeasonId = playerDto.SeasonId ?? player.SeasonId;
                player.Position = playerDto.Position ?? player.Position;
                player.JoinDate = playerDto.JoinDate ?? player.JoinDate;
                player.OutDate = playerDto.OutDate ?? player.OutDate;
                player.Description = playerDto.Description ?? player.Description;
                player.BackShirtImage = playerDto.BackShirtImage ?? player.BackShirtImage;
                player.Status = playerDto.Status ?? player.Status;

                await _context.SaveChangesAsync();

                return playerDto;
            }
            catch (Exception ex)
            {
                throw new Exception("không cập nhật được cầu thủ", ex);
            }
        }
    }
}
