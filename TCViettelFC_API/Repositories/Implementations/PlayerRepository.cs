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
                    avatar = playerDto.avatar,
                    Status = playerDto.Status,
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
            // Tìm cầu thủ trong cơ sở dữ liệu
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                // Ném ngoại lệ nếu không tìm thấy cầu thủ
                throw new KeyNotFoundException("không thấy cầu thủ");
            }

            // Thực hiện soft delete
            player.Status = 0; // Đánh dấu trạng thái là "đã xóa"
            await _context.SaveChangesAsync();

            // Trả về thông tin cầu thủ đã xóa
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
                avatar = player.avatar,
                Status = player.Status
            };
        }


        public async Task<PlayerDto> GetPlayerByIdAsync(int id)
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
                    Status = player.Status
                };
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
                        avatar = player.avatar,
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
            // Tìm cầu thủ trong cơ sở dữ liệu
            var player = await _context.Players.FindAsync(playerDto.PlayerId);
            if (player == null)
            {
                // Ném ngoại lệ nếu không tìm thấy cầu thủ
                throw new KeyNotFoundException("không tìm được cầu thủ với id đó");
            }

            // Cập nhật thông tin cầu thủ
            player.FullName = playerDto.FullName ?? player.FullName;
            player.ShirtNumber = playerDto.ShirtNumber ?? player.ShirtNumber;
            player.SeasonId = playerDto.SeasonId ?? player.SeasonId;
            player.Position = playerDto.Position ?? player.Position;
            player.JoinDate = playerDto.JoinDate ?? player.JoinDate;
            player.OutDate = playerDto.OutDate ?? player.OutDate;
            player.Description = playerDto.Description ?? player.Description;
            player.avatar = playerDto.avatar ?? player.avatar;
            player.Status = playerDto.Status ?? player.Status;

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Trả về đối tượng PlayerDto sau khi cập nhật
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
                avatar = player.avatar,
                Status = player.Status
            };
        }

    }
}
