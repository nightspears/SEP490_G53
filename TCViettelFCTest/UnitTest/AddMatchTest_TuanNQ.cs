using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Controllers;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using CloudinaryDotNet;
using static QRCoder.PayloadGenerator.SwissQrCode;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TCViettelFC_API.Dtos.Matches;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class AddMatchtTest_TuanNQ
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;
        // cái đầu tiên chay 
        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;
         Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");


        }

        private Mock<ICloudinarySetting> _mockCloudinary;

        [SetUp]
        public void SetupMocks()
        {
            _mockCloudinary = new Mock<ICloudinarySetting>();

            // Mock upload avatar
            _mockCloudinary.Setup(c => c.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("http://example.com/avatar.jpg") });

            // Mock upload file in DataFile
            _mockCloudinary.Setup(c => c.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("http://example.com/file.jpg") });
        }

        [TearDown]
        public void TearDown()
        {
            _dbContextOptions = null; // Giải phóng tùy chọn
        }
        [Test]
        public async Task AddMatchAsync_ShouldAddMatch()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = true,
                MatchDate = DateTime.Parse("10/12/2024"),
                StadiumName = "Hàng Đẫy",
                Status = 1,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),
              
            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            await repository.AddMatchesAsync(matchtDto);
            await context.SaveChangesAsync();

            var savedMatch = await context.Matches.FirstOrDefaultAsync(p => p.Id == 1 );
            Assert.IsNotNull(savedMatch);
            Assert.AreEqual("Hà Nội FC", savedMatch.OpponentName);

        }
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenOpponentNameIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = null,
                IsHome = true,
                MatchDate = DateTime.Parse("25/12/2024"),
                StadiumName = "Hàng Đẫy",
                Status = 1,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),
            
        };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddMatchesAsync(matchtDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new match are added.", exception.Message);
        }
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenisHomeIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = null,
                MatchDate = DateTime.Parse("25/12/2024"),
                StadiumName = "Hàng Đẫy",
                Status = 1,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddMatchesAsync(matchtDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new match are added.", exception.Message);
        }
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenStadiumNameIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = false,
                MatchDate = DateTime.Parse("25/12/2024"),
                StadiumName = null,
                Status = 1,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddMatchesAsync(matchtDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new match are added.", exception.Message);
        }
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenMatchDateIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
              .UseInMemoryDatabase("TestDatabase")
              .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = false,
                MatchDate = null,
                StadiumName = "Hàng Đẫy",
                Status = 1,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddMatchesAsync(matchtDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new match are added.", exception.Message);
        }
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenAllIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
              .UseInMemoryDatabase("TestDatabase")
              .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = null,
                IsHome = null,
                MatchDate = null,
                StadiumName = null,
                Status = null,
                LogoUrl =null,

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddMatchesAsync(matchtDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new match are added.", exception.Message);
        } 
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenMatchDatelessThanToday()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
              .UseInMemoryDatabase("TestDatabase")
              .Options;

            using var context = new Sep490G53Context(options);

            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = true,
                MatchDate = DateTime.Parse("25/12/2024"),
                StadiumName = "Hàng Đẫy",
                Status = 1,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddMatchesAsync(matchtDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("match date must greater than today", exception.Message);
        }
        [Test]
        public async Task AddMatchAsync_ShouldAdded_WhenStatusIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);
            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = true,
                MatchDate = DateTime.Parse("25/12/2024"),
                StadiumName = "Hàng Đẫy",
                Status = null,
                LogoUrl = CreateMockFile("logo.jpg", new byte[] { 1 }),

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            await repository.AddMatchesAsync(matchtDto);
            await context.SaveChangesAsync();

            var savedMatch = await context.Matches.FirstOrDefaultAsync(p => p.Id == 1);
            Assert.IsNotNull(savedMatch);
            Assert.AreEqual("Hà Nội FC", savedMatch.OpponentName);
        }
        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenLogoIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
             .UseInMemoryDatabase("TestDatabase")
             .Options;

            using var context = new Sep490G53Context(options);
            var matchtDto = new MatchesAddDto
            {
                OpponentName = "Hà Nội FC",
                IsHome = true,
                MatchDate = DateTime.Parse("25/12/2024"),
                StadiumName = "Hàng Đẫy",
                Status = 1,
                LogoUrl = null,

            };

            var repository = new MatchRepository(context, _mockCloudinary.Object);

            await repository.AddMatchesAsync(matchtDto);
            await context.SaveChangesAsync();

            var savedMatch = await context.Matches.FirstOrDefaultAsync(p => p.Id == 1);
            Assert.IsNotNull(savedMatch);
            Assert.AreEqual("Hà Nội FC", savedMatch.OpponentName);
        }

        private IFormFile CreateMockFile(string fileName, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }



    }


}