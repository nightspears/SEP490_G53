using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TCViettelFC_API.Repositories.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    internal class Customer_LoginTest_AnSTH
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IHttpContextAccessor> _mockContextAccessor;

        [SetUp]
        public void Setup()
        {
            // Setup in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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

        [Test]
        // Test Case 1: Valid credentials
        public async Task LoginAsync_TC1()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);

            // Create a test customer account
            var customer = new CustomersAccount
            {
                CustomerId = 1,
                Email = "test@example.com",
                Password = HashPassword("password123"),
                Phone = "1234567890"
            };

            context.CustomersAccounts.Add(customer);
            await context.SaveChangesAsync();  // Ensure the customer is saved

            // Verify the customer is saved
            var savedCustomer = await context.CustomersAccounts
                .FirstOrDefaultAsync(x => x.Email == customer.Email);
            Assert.IsNotNull(savedCustomer, "Customer should be saved to the database.");
            Assert.AreEqual(customer.Email, savedCustomer.Email, "Customer's email should match.");

            // Mock HttpContextAccessor
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Name, "TestUser")})));
            _mockContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            // Initialize CustomerRepository
            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);

            var loginDto = new CustomerLoginDto
            {
                Email = "test@example.com",
                Password = "password123",
            };

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNotNull(result, "LoginAsync returned null, check if customer exists, password matches, or JWT creation fails.");
            Assert.AreEqual(customer.CustomerId, result.customerId);
            Assert.AreEqual(customer.Email, result.email);
            Assert.AreEqual(customer.Phone, result.phone);
            Assert.IsNotEmpty(result.token);
        }
        // Test Case 2: Invalid credentials (email null)
        [Test]
        public async Task LoginAsync_TC2()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);

            // Create a test customer account
            var customer = new CustomersAccount
            {
                CustomerId = 1,
                Email = "test@example.com",
                Password = HashPassword("password123"),
                Phone = "1234567890"
            };
            context.CustomersAccounts.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);

            var loginDto = new CustomerLoginDto
            {
                Email = "",
                Password = "wrongpassword"
            };

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
            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);
            var loginDto = new CustomerLoginDto
            {
                Email = "wrongformat",
                Password = "password123"
            };

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
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);
            var loginDto = new CustomerLoginDto
            {
                Email = "",
                Password = "password123"
            };

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }
        //tes case 5: Invalid credentials (eamil wrongformat (1))
        [Test]
        public async Task LoginAsync_TC5()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var customer = new CustomersAccount
            {
                CustomerId = 1,
                Email = "test@example.com",
                Password = HashPassword("password123"),
                Phone = "1234567890"
            };

            context.CustomersAccounts.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);
            var loginDto = new CustomerLoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.token);
            var expiration = jwtToken.ValidTo;
            Assert.AreEqual(DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), expiration.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        //tes case 7: Invalid credentials (password wroong)
        [Test]
        public async Task LoginAsync_TC7()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var customer = new CustomersAccount
            {
                CustomerId = 1,
                Email = "test@example.com",
                Password = HashPassword("password123"),
                Phone = "1234567890"
            };

            context.CustomersAccounts.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);
            var loginDto = new CustomerLoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.token);
            var expiration = jwtToken.ValidTo;
            Assert.AreEqual(DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), expiration.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        //tes case 8: Invalid credentials (passwrod wrong format)
        [Test]
        public async Task LoginAsync_TC8()
        {
            // Arrange
            using var context = new Sep490G53Context(_dbContextOptions);
            var customer = new CustomersAccount
            {
                CustomerId = 1,
                Email = "test@example.com",
                Password = HashPassword("password123"),
                Phone = "1234567890"
            };

            context.CustomersAccounts.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context, _mockConfiguration.Object, _mockEmailService.Object, _mockContextAccessor.Object);
            var loginDto = new CustomerLoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await repository.LoginAsync(loginDto);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.token);
            var expiration = jwtToken.ValidTo;
            Assert.AreEqual(DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), expiration.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
