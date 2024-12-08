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
using TCViettelFC_API.Dtos.Discount;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class AddDiscountTest_TuanNQ
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
        public async Task AddDiscountAsync_ShouldAddDiscount()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("12/12/2024"),
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = 1,
              
            };

            var repository = new DiscoutRepository(context);

            await repository.AddDiscountAsync(discounttDto);
            await context.SaveChangesAsync();

            var savedDiscount = await context.Discounts.FirstOrDefaultAsync(p => p.DiscountId == 1 );
            Assert.IsNotNull(savedDiscount);
            Assert.AreEqual("Mã giảm giá 20% - Sale 12/12", savedDiscount.DiscountName);

        }


        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenDiscountNameIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = null,
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("12/12/2024"),
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("The system returns an error, no new Discount are added.", exception.Message);
        }


        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenDiscountPercentIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = null,
                ValidFrom = DateTime.Parse("12/12/2024"),
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("The system returns an error, no new Discount are added.", exception.Message);
        }
        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenValidFromIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = null,
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("The system returns an error, no new Discount are added.", exception.Message);
        }

        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenValidFromAnndValidUntilIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = null,
                ValidUntil = null,
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("The system returns an error, no new Discount are added.", exception.Message);
        }
        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenValidUntilIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("12/12/2024"),
                ValidUntil = null ,
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("The system returns an error, no new Discount are added.", exception.Message);
        }


        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenValidFromLessThanToday()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("7/12/2024"),
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("valid_from must greater than today", exception.Message);
        }

        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenValidUltilLessThanToday()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("12/12/2024"),
                ValidUntil = DateTime.Parse("7/12/2024"),
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("valid_until must greater than today", exception.Message);
        }

        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenValidFromGreaterThanValidUntil()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("14/12/2024"),
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = 1,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            Assert.AreEqual("valid_from must less than valid_until", exception.Message);
        }

       
        [Test]
        public async Task AddDiscountAsync_ShouldThrowException_WhenAllIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName =null,
                DiscountPercent = null,
                ValidFrom = null,
                ValidUntil = null,
                Status = null,

            };

            var repository = new DiscoutRepository(context);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddDiscountAsync(discounttDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no new Discount are added.", exception.Message);
        } 
   
        [Test]
        public async Task AddDiscountAsync_ShouldAdded_WhenStatusIsNull()
        {
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            var discounttDto = new DiscountDto
            {
                DiscountName = "Mã giảm giá 20% - Sale 12/12",
                DiscountPercent = 20,
                ValidFrom = DateTime.Parse("12/12/2024"),
                ValidUntil = DateTime.Parse("13/12/2024"),
                Status = null,

            };

            var repository = new DiscoutRepository(context);

            await repository.AddDiscountAsync(discounttDto);
            await context.SaveChangesAsync();

            var savedDiscount = await context.Discounts.FirstOrDefaultAsync(p => p.DiscountId == 1);
            Assert.IsNotNull(savedDiscount);
            Assert.AreEqual("Mã giảm giá 20% - Sale 12/12", savedDiscount.DiscountName);
        }





    }


}