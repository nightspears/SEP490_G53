using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TCViettelFCTest.UnitTest;
using System.Security.Cryptography;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Implementations;

// Assuming the namespace for your models
namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    internal class LoginTest_AnSTH
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IHttpContextAccessor> _mockContextAccessor;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            // Setup mock configuration for JWT
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["JwtConfig:Issuer"]).Returns("FA24_SEP490_G53");
            _mockConfiguration.Setup(c => c["JwtConfig:Audience"]).Returns("FA24_SEP490_G53");
            _mockConfiguration.Setup(c => c["JwtConfig:Key"]).Returns("FA24_SEP490_G53#FA24_SEP490_G53#FAFA24_SEP490_G53#FA24_SEP490_G53#FAFA24_SEP490_G53#FA24_SEP490_G53#FAFA24_SEP490_G53#FA24_SEP490_G53#FA");
            // Mock the email service
            _mockEmailService = new Mock<IEmailService>();
            _mockEmailService.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(Task.CompletedTask);

            // Mock the HttpContextAccessor
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            return Convert.ToBase64String(hashBytes);
        }

        // Test Case 1: Valid credentials
        [Test]
        public async Task LoginAsync_TC1()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"),
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "staff@example.com",
                Password = "password123"
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserId, result.UserId);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.Phone, result.Phone);
            Assert.AreEqual(user.RoleId, result.RoleId);
            Assert.AreEqual(user.FullName, result.FullName);
            Assert.AreEqual(user.Status, result.Status);
            Assert.IsNotEmpty(result.Token); // Ensure a token is generated
        }

        // Test Case 2: Invalid credentials (email null)
        [Test]
        public async Task LoginAsync_TC2()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "",
                Password = "password123"
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 3: Invalid credentials (eamil wrong format)
        [Test]
        public async Task LoginAsync_TC3()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "staffexample",
                Password = "password123"
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 4: Invalid credentials (eamil not exist)
        [Test]
        public async Task LoginAsync_TC4()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            var loginDto = new LoginDto
            {
                Email = "staff@example.com",
                Password = "password123"
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 5: Invalid credentials (password null)
        [Test]
        public async Task LoginAsync_TC5()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "staff@example.com",
                Password = ""
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 6: Invalid credentials (password wrong)
        [Test]
        public async Task LoginAsync_TC6()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "staff@example.com",
                Password = "wrongpassword"
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 7: Invalid credentials (password wroong)
        [Test]
        public async Task LoginAsync_TC7()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "staff@example.com",
                Password = "@@@@@@@@@@@@@"
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 8: Invalid credentials (passwrod wrong format)
        [Test]
        public async Task LoginAsync_TC8()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var user = new User
            {
                UserId = 1,
                Email = "staff123@example.com",
                Password = HashPassword("password123"), // Assuming you have a HashPassword method
                Phone = "1234567890",
                RoleId = 2,
                FullName = "John Doe",
                Status = 1
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
            };

            var repository = new UserRepository(_mockEmailService.Object, context, _mockConfiguration.Object, _mockContextAccessor.Object);

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }


    }
}
