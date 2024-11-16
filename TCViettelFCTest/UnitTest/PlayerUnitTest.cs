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

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class PlayerUnitTest
    {
        private Mock<IPlayerRepository> _playerRepositoryMock;
        private PlayersController _playersController;

        [SetUp]
        public void SetUp()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _playersController = new PlayersController(_playerRepositoryMock.Object);
        }
        //test add hoan chinh
        [Test]
        public async Task AddPlayerAsync_UnitTest()
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
                BackShirtImage = null,
                avatar = null
            };

            // Mock repository
            _playerRepositoryMock.Setup(repo => repo.AddPlayerAsync(It.IsAny<PlayerDto>())).ReturnsAsync(playerDto);

            // Act
            var result = await _playersController.AddPlayer(playerDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);

            var returnedPlayerDto = createdAtActionResult.Value as PlayerDto;
            Assert.AreEqual(playerDto.FullName, returnedPlayerDto.FullName);
            Assert.AreEqual(playerDto.ShirtNumber, returnedPlayerDto.ShirtNumber);
            Assert.AreEqual(playerDto.SeasonId, returnedPlayerDto.SeasonId);
            Assert.AreEqual(playerDto.Position, returnedPlayerDto.Position);
        }

        //test list tat ca
        [Test]
        public async Task GetAllPlayers_UnitTest()
        {
            // Arrange
            var playersList = new List<PlayerDto>
            {
                new PlayerDto { FullName = "Nguyễn Hoàng Đức", ShirtNumber = 14, Position = "Tiền đạo" },
                new PlayerDto { FullName = "Trần Minh Tuấn", ShirtNumber = 7, Position = "Hậu vệ" }
            };

            // Mock repository
            _playerRepositoryMock.Setup(repo => repo.ListAllPlayerAsync()).ReturnsAsync(playersList);

            // Act
            var result = await _playersController.GetAllPlayers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedPlayersList = okResult.Value as List<PlayerDto>;
            Assert.AreEqual(playersList.Count, returnedPlayersList.Count);
            Assert.AreEqual(playersList[0].FullName, returnedPlayersList[0].FullName);
        }
        [Test]
        public async Task GetPlayerById_UnitTest()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                FullName = "Nguyễn Hoàng Đức",
                ShirtNumber = 14,
                Position = "Tiền đạo"
            };

            // Mock repository
            _playerRepositoryMock.Setup(repo => repo.GetPlayerByIdAsync(1)).ReturnsAsync(playerDto);

            // Act
            var result = await _playersController.GetPlayerById(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedPlayerDto = okResult.Value as PlayerDto;
            Assert.AreEqual(playerDto.FullName, returnedPlayerDto.FullName);
            Assert.AreEqual(playerDto.ShirtNumber, returnedPlayerDto.ShirtNumber);
        }


    }
}
