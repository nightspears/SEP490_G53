using CloudinaryDotNet.Actions;
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
        private readonly ICloudinarySetting _cloudinary;
        public PlayerRepository(Sep490G53Context context, ICloudinarySetting cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<PlayerDto> AddPlayerAsync(PlayerDto playerDto)
        {
            try
            {
                var player = new Player();
                player.FullName = playerDto.FullName;
                player.ShirtNumber = playerDto.ShirtNumber;
                player.SeasonId = playerDto.SeasonId;
                player.Position = playerDto.Position;
                player.JoinDate = playerDto.JoinDate;
                player.OutDate = playerDto.OutDate;
                player.Description = playerDto.Description;
                if (playerDto.avatar != null && playerDto.avatar.Length > 0)
                {
                    ImageUploadResult res = _cloudinary.CloudinaryUpload(playerDto.avatar);
                    player.avatar = res.SecureUrl.ToString();
                }
                else
                {
                    player.avatar = "/image/imagelogo/ImageFail.jpg";
                }
                player.Status = playerDto.Status;

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
                throw new KeyNotFoundException("không tìm được cầu thủ với id đó");
            }

            player.Status = 0;
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
                        Status = player.Status
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("không liệt kê được cầu thủ", ex);
            }
        }

        public async Task<PlayerDto> UpdatePlayerAsync(int id, PlayerDto playerDto)
        {
            using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Fetch player from the database
                    var player = await _context.Players.FindAsync(id);
                    if (player == null)
                    {
                        throw new KeyNotFoundException("Không tìm được cầu thủ với id đó");
                    }

                    // Update player properties
                    player.FullName = playerDto.FullName ?? player.FullName;
                    player.ShirtNumber = playerDto.ShirtNumber ?? player.ShirtNumber;
                    player.SeasonId = playerDto.SeasonId ?? player.SeasonId;
                    player.Position = playerDto.Position ?? player.Position;
                    player.JoinDate = playerDto.JoinDate ?? player.JoinDate;
                    player.OutDate = playerDto.OutDate ?? player.OutDate;
                    player.Description = playerDto.Description ?? player.Description;

                    // Handle avatar upload
                    if (playerDto.avatar != null && playerDto.avatar.Length > 0)
                    {
                        ImageUploadResult res = _cloudinary.CloudinaryUpload(playerDto.avatar);
                        player.avatar = res.SecureUrl.ToString();
                    }
                    else
                    {
                        player.avatar = "/image/imagelogo/ImageFail.jpg";
                    }

                    player.Status = playerDto.Status ?? player.Status;

                    // Save changes
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await dbContextTransaction.CommitAsync();

                    // Return the updated PlayerDto
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
                catch (Exception)
                {
                    // Rollback transaction in case of an error
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
        }


    }
}
