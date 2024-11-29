using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ShowPlayerDtos>> ListAllPlayerAsync()
        {
            var players = await _context.Players
                .Include(p => p.Season)
                .Where(p=>p.Status!= 0)
                .ToListAsync();

            return players.Select(p => new ShowPlayerDtos
            {
                PlayerId = p.PlayerId,
                FullName = p.FullName,
                ShirtNumber = p.ShirtNumber,
                Position = p.Position,
                JoinDate = p.JoinDate,
                OutDate = p.OutDate,
                Description = p.Description,
                Status = p.Status,
                Avatar = p.avatar,
                SeasonId = p.SeasonId,
                SeasonName = p.Season?.SeasonName
            }).ToList();
        }
        public async Task<List<ShowPlayerDtos>> ListAllPlayerActiveAsync()
        {
            var players = await _context.Players
                .Include(p => p.Season)
                .Where(p => p.Status == 1)
                .ToListAsync();

            return players.Select(p => new ShowPlayerDtos
            {
                PlayerId = p.PlayerId,
                FullName = p.FullName,
                ShirtNumber = p.ShirtNumber,
                Position = p.Position,
                JoinDate = p.JoinDate,
                OutDate = p.OutDate,
                Description = p.Description,
                Status = p.Status,
                Avatar = p.avatar,
                SeasonId = p.SeasonId,
                SeasonName = p.Season?.SeasonName
            }).ToList();
        }

        public async Task<ShowPlayerDtos> GetPlayerByIdAsync(int id)
        {
            var player = await _context.Players
                .Include(p => p.Season)
                .FirstOrDefaultAsync(p => p.PlayerId == id);

            if (player == null)
                return null;

            return new ShowPlayerDtos
            {
                PlayerId = player.PlayerId,
                FullName = player.FullName,
                ShirtNumber = player.ShirtNumber,
                Position = player.Position,
                JoinDate = player.JoinDate,
                OutDate = player.OutDate,
                Description = player.Description,
                Status = player.Status,
                Avatar = player.avatar,
                SeasonId = player.SeasonId,
                SeasonName = player.Season?.SeasonName
            };
        }

        public async Task<ShowPlayerDtos> AddPlayerAsync(PlayerDto playerDtoInput)
        {
            var player = new Player
            {
                FullName = playerDtoInput.FullName,
                ShirtNumber = playerDtoInput.ShirtNumber,
                Position = playerDtoInput.Position,
                SeasonId = playerDtoInput.SeasonId,
                JoinDate = playerDtoInput.JoinDate,
                OutDate = playerDtoInput.OutDate,
                Description = playerDtoInput.Description,
                Status = playerDtoInput.Status,
            };

            // Upload avatar if provided
            if (playerDtoInput.Avatar != null && playerDtoInput.Avatar.Length > 0)
            {
                var res = _cloudinary.CloudinaryUpload(playerDtoInput.Avatar);
                player.avatar = res.SecureUrl.ToString();
            }

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return new ShowPlayerDtos
            {
                PlayerId = player.PlayerId,
                FullName = player.FullName,
                ShirtNumber = player.ShirtNumber,
                Position = player.Position,
                JoinDate = player.JoinDate,
                OutDate = player.OutDate,
                Description = player.Description,
                Status = player.Status,
                Avatar = player.avatar,
                SeasonId = player.SeasonId,
                SeasonName = player.Season?.SeasonName,
            };
        }

        public async Task<ShowPlayerDtos> UpdatePlayerAsync(int id, PlayerDto playerDtoInput)
        {
            var player = await _context.Players
                .Include(p => p.Season)
                .FirstOrDefaultAsync(p => p.PlayerId == id);
            if (player == null)
                throw new Exception("Player not found");

            // Update fields
            player.FullName = playerDtoInput.FullName;
            player.ShirtNumber = playerDtoInput.ShirtNumber;
            player.Position = playerDtoInput.Position;
            player.JoinDate = playerDtoInput.JoinDate;
            player.OutDate = playerDtoInput.OutDate;
            player.Description = playerDtoInput.Description;
            player.Status = playerDtoInput.Status;
            player.SeasonId = playerDtoInput.SeasonId;

            // Update avatar if provided
            if (playerDtoInput.Avatar != null && playerDtoInput.Avatar.Length > 0)
            {
                var res = _cloudinary.CloudinaryUpload(playerDtoInput.Avatar);
                player.avatar = res.SecureUrl.ToString();
            }

            _context.Players.Update(player);
            await _context.SaveChangesAsync();

            return new ShowPlayerDtos
            {
                PlayerId = player.PlayerId,
                FullName = player.FullName,
                ShirtNumber = player.ShirtNumber,
                Position = player.Position,
                JoinDate = player.JoinDate,
                OutDate = player.OutDate,
                Description = player.Description,
                Status = player.Status,
                Avatar = player.avatar,
                SeasonId = player.SeasonId,
                SeasonName = player.Season.SeasonName,
            };
        }


        public async Task<ShowPlayerDtos> DeletePlayerAsync(int id)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == id);
            if (player == null)
                throw new Exception("Không tìm thấy người chơi");
            player.Status = 0;

            _context.Players.Update(player);
            await _context.SaveChangesAsync();

            return new ShowPlayerDtos
            {
                PlayerId = player.PlayerId,
                FullName = player.FullName,
                ShirtNumber = player.ShirtNumber,
                Position = player.Position,
                JoinDate = player.JoinDate,
                OutDate = player.OutDate,
                Description = player.Description,
                Status = player.Status,
                Avatar = player.avatar,
                SeasonName = player.Season?.SeasonName
            };
        }
    }
}
