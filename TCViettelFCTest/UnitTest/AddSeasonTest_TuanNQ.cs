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
using TCViettelFC_API.Dtos.Season;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class AddSeasonTest_TuanNQ
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
        public async Task AddSeasonAsync_ShouldAddSeason()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = DateTime.Parse("25/9/2022"),
                EndYear = DateTime.Parse("25/9/2023"),
                Status = 1,
              
            };
            var repository = new SeasonRepository(context);

            await repository.AddSeasonAsync(seasonDto);
            await context.SaveChangesAsync();

            var savedSeason= await context.Seasons.FirstOrDefaultAsync(p => p.SeasonId == 1 );
            Assert.IsNotNull(savedSeason);
            Assert.AreEqual("Mùa giải 2022 - 2023", savedSeason.SeasonName);

        }
        [Test]
        public async Task AddSeasonAsync_ShouldThrowException_WhenSeasonNameIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = null,
                StartYear = DateTime.Parse("25/9/2022"),
                EndYear = DateTime.Parse("25/9/2023"),
                Status = 1,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }
        [Test]
        public async Task AddSeasonAsync_ShouldThrowException_WhenStartYearIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = null,
                EndYear = DateTime.Parse("25/9/2023"),
                Status = 1,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }
        [Test]
        public async Task AddSeasonAsync_ShouldThrowException_WhenEndYearIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = DateTime.Parse("25/9/2022"),
                EndYear = null,
                Status = 1,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }
        [Test]
        
        public async Task AddSeasonAsync_ShouldThrowException_WhenStartYearGreaterEndYear()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = DateTime.Parse("25/9/2023"),
                EndYear = DateTime.Parse("25/9/2022"),
                Status = 1,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("start_year must greater than end_year", exception.Message);
        }
        [Test]
        public async Task AddMatchAsync_ShouldThrowException_WhenAllIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = null,
                StartYear = null,
                EndYear = null,
                Status = null,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }
        [Test]
        public async Task AddSeasonAsync_ShouldThrowException_WhenStartYearAndStatusIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = null,
                EndYear = DateTime.Parse("25/9/2023"),
                Status = null,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }
        [Test]
        public async Task AddSeasonAsync_ShouldThrowException_WhenEndYearAndStatusIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = null,
                EndYear = DateTime.Parse("25/9/2023"),
                Status = null,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }
        [Test]
        public async Task AddSeasonAsync_ShouldThrowException_WhenStartYearAndSeasonNameIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                 .UseInMemoryDatabase("TestDatabase")
                 .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = null,
                StartYear = null,
                EndYear = DateTime.Parse("25/9/2023"),
                Status = 1,

            };
            var repository = new SeasonRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddSeasonAsync(seasonDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new season are added.", exception.Message);
        }

        [Test]
        public async Task AddSeasonAsync_ShouldAddSeason_whenStatusNULL()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            var seasonDto = new SeasonDto
            {
                SeasonName = "Mùa giải 2022 - 2023",
                StartYear = DateTime.Parse("25/9/2022"),
                EndYear = DateTime.Parse("25/9/2023"),
                Status = null,

            };
            var repository = new SeasonRepository(context);

            await repository.AddSeasonAsync(seasonDto);
            await context.SaveChangesAsync();

            var savedSeason = await context.Seasons.FirstOrDefaultAsync(p => p.SeasonId == 1);
            Assert.IsNotNull(savedSeason);
            Assert.AreEqual("Mùa giải 2022 - 2023", savedSeason.SeasonName);

        }




    }


}