using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Controllers;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using TCViettelFC_API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Internal;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class PlayerUnitTests
    {
        private DbContextOptions<Sep490G53Context> _options;
        private Sep490G53Context _context;
        private PlayerRepository _playerRepository;

        [SetUp]
        public void SetUp()
        {
            // Use an in-memory database
            _options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Sep490G53Context(_options);
            _playerRepository = new PlayerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddPlayer_Success()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                FullName = "Nguyễn Hoàng Đức",
                ShirtNumber = 14,
                SeasonId = 1,
                Position = "Tiền đạo",
                JoinDate = DateTime.Parse("2022-05-19"),
                Status = 1,
                Description = "Cầu thủ thuận cả 2 chân",
                avatar = "img_avatar_location"
            };

            // Act
            var result = await _playerRepository.AddPlayerAsync(playerDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(playerDto.FullName, result.FullName);
            Assert.AreEqual(playerDto.ShirtNumber, result.ShirtNumber);

            var savedPlayer = await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == result.PlayerId);
            Assert.IsNotNull(savedPlayer);
            Assert.AreEqual(playerDto.Position, savedPlayer.Position);
        }

        [Test]
        public async Task GetPlayerById_Success()
        {
            // Arrange
            var player = new Player
            {
                FullName = "Nguyễn Hoàng Đức",
                ShirtNumber = 14,
                Position = "Tiền đạo",
                JoinDate = DateTime.Parse("2022-05-19"),
                Status = 1
            };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            // Act
            var result = await _playerRepository.GetPlayerByIdAsync(player.PlayerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(player.FullName, result.FullName);
            Assert.AreEqual(player.ShirtNumber, result.ShirtNumber);
        }

        [Test]
        public async Task GetPlayerById_NotFound_ThrowsException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _playerRepository.GetPlayerByIdAsync(99));
            Assert.AreEqual("không tìm được cầu thủ với id đó", ex.Message);
        }

        [Test]
        public async Task ListAllPlayerAsync_ReturnsAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { FullName = "Nguyễn Hoàng Đức", ShirtNumber = 14, Position = "Tiền đạo", Status = 1 },
                new Player { FullName = "Trần Minh Tuấn", ShirtNumber = 7, Position = "Hậu vệ", Status = 1 }
            };
            _context.Players.AddRange(players);
            await _context.SaveChangesAsync();

            // Act
            var result = await _playerRepository.ListAllPlayerAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(players.Count, result.Count);
            Assert.AreEqual(players[0].FullName, result[0].FullName);
        }

        [Test]
        public async Task DeletePlayer_Success()
        {
            // Arrange
            var player = new Player
            {
                FullName = "Nguyễn Hoàng Đức",
                ShirtNumber = 14,
                Position = "Tiền đạo",
                Status = 1
            };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            // Act
            var result = await _playerRepository.DeletePlayerAsync(player.PlayerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, (await _context.Players.FindAsync(player.PlayerId)).Status); // Ensure status is updated
        }

        //[Test]
        //public async Task DeletePlayer_NotFound_ThrowsException()
        //{
        //    // Act & Assert
        //    var ex = await _playerRepository.DeletePlayerAsync(99);
        //    Assert.Throws<KeyNotFoundException>(ex);
        //    var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _playerRepository.DeletePlayerAsync(99));
        //    Assert.AreEqual("không thấy cầu thủ", ex.Message);
        //}

        [Test]
        public async Task UpdatePlayer_Success()
        {
            // Arrange
            var player = new Player
            {
                FullName = "Nguyễn Hoàng Đức",
                ShirtNumber = 14,
                Position = "Tiền đạo",
                Status = 1
            };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var updatedPlayerDto = new PlayerDto
            {
                PlayerId = player.PlayerId,
                FullName = "Cập nhật Hoàng Đức",
                ShirtNumber = 10,
                Position = "Hậu vệ",
                Status = 1
            };

            // Act
            var result = await _playerRepository.UpdatePlayerAsync(updatedPlayerDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedPlayerDto.FullName, result.FullName);
            Assert.AreEqual(updatedPlayerDto.ShirtNumber, result.ShirtNumber);
        }

        [Test]
        public async Task UpdatePlayer_PlayerNotFound_ThrowsException()
        {
            // Arrange
            var updatedPlayerDto = new PlayerDto
            {
                PlayerId = 99,
                FullName = "Cập nhật Hoàng Đức",
                ShirtNumber = 10,
                Position = "Hậu vệ",
                Status = 1
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _playerRepository.UpdatePlayerAsync(updatedPlayerDto));
            Assert.AreEqual("không tìm được cầu thủ để cập nhật", ex.Message);
        }

    }
}