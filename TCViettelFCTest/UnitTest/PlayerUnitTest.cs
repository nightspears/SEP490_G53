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
using CloudinaryDotNet.Actions;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class PlayerUnitTests
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;
        private Mock<ICloudinarySetting> _mockCloudinary;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockCloudinary = new Mock<ICloudinarySetting>();
        }

        [Test]
        public async Task ListAllPlayerAsync_ShouldReturnAllPlayers()
        {
            using var context = new Sep490G53Context(_dbContextOptions);
            context.Players.Add(new Player { PlayerId = 1, FullName = "Player 1", Status = 1 });
            context.Players.Add(new Player { PlayerId = 2, FullName = "Player 2", Status = 0 });
            await context.SaveChangesAsync();

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.ListAllPlayerAsync();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(p => p.FullName == "Player 1"));
            Assert.IsTrue(result.Any(p => p.FullName == "Player 2"));
        }

        [Test]
        public async Task ListAllPlayerActiveAsync_ShouldReturnOnlyActivePlayers()
        {
            using var context = new Sep490G53Context(_dbContextOptions);
            context.Players.Add(new Player { PlayerId = 1, FullName = "Player 1", Status = 1 });
            context.Players.Add(new Player { PlayerId = 2, FullName = "Player 2", Status = 0 });
            await context.SaveChangesAsync();

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.ListAllPlayerActiveAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Player 1", result[0].FullName);
        }

        [Test]
        public async Task GetPlayerByIdAsync_ShouldReturnPlayer_WhenPlayerExists()
        {
            using var context = new Sep490G53Context(_dbContextOptions);
            var player = new Player { PlayerId = 1, FullName = "Player 1", Status = 1 };
            context.Players.Add(player);
            await context.SaveChangesAsync();

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.GetPlayerByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Player 1", result.FullName);
        }

        [Test]
        public async Task GetPlayerByIdAsync_ShouldReturnNull_WhenPlayerDoesNotExist()
        {
            using var context = new Sep490G53Context(_dbContextOptions);

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.GetPlayerByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task AddPlayerAsync_ShouldAddPlayer()
        {
            using var context = new Sep490G53Context(_dbContextOptions);
            var playerDto = new PlayerDto
            {
                FullName = "New Player",
                ShirtNumber = 10,
                Position = "Forward",
                Status = 1,
                JoinDate = DateTime.Now,
                Description = "Test player"
            };

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.AddPlayerAsync(playerDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("New Player", result.FullName);
        }

        [Test]
        public async Task UpdatePlayerAsync_ShouldUpdatePlayer()
        {
            using var context = new Sep490G53Context(_dbContextOptions);
            var player = new Player { PlayerId = 1, FullName = "Player 1", Status = 1 };
            context.Players.Add(player);
            await context.SaveChangesAsync();

            var playerDto = new PlayerDto
            {
                FullName = "Updated Player 1",
                ShirtNumber = 20,
                Position = "Midfielder",
                Status = 1,
                JoinDate = DateTime.Now,
                Description = "Updated player"
            };

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.UpdatePlayerAsync(1, playerDto);

            Assert.AreEqual("Updated Player 1", result.FullName);
            Assert.AreEqual(20, result.ShirtNumber);
        }

        [Test]
        public async Task DeletePlayerAsync_ShouldDeletePlayer()
        {
            using var context = new Sep490G53Context(_dbContextOptions);
            var player = new Player { PlayerId = 1, FullName = "Player 1", Status = 1 };
            context.Players.Add(player);
            await context.SaveChangesAsync();

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var result = await repository.DeletePlayerAsync(1);

            Assert.AreEqual("Player 1", result.FullName);
            Assert.AreEqual(0, context.Players.Count());
        }

        [Test]
        public async Task DeletePlayerAsync_ShouldThrowException_WhenPlayerNotFound()
        {
            using var context = new Sep490G53Context(_dbContextOptions);

            var repository = new PlayerRepository(context, _mockCloudinary.Object);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repository.DeletePlayerAsync(999));

            Assert.AreEqual("không tìm thấy người chơi", ex.Message);
        }
    }
}