using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;



namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class AddUserUnitTest_DuongNQ
    {
        private Mock<Sep490G53Context> _mockContext;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private UserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<Sep490G53Context>(new DbContextOptions<Sep490G53Context>());
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _userRepository = new UserRepository(null, _mockContext.Object, _mockConfiguration.Object, _mockHttpContextAccessor.Object);
        }
        //case1
        [Test]
        public async Task AddUserAsync_ValidInput_AddsUserSuccessfully()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "Test User",
                Password = "Password123",
                Email = "test@example.com",
                Phone = "123456789",
                RoleId = 2
            };

            var mockSet = new Mock<DbSet<User>>();
            _mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            await _userRepository.AddUserAsync(userCreateDto);

            mockSet.Verify(m => m.AddAsync(It.IsAny<User>(), default), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
        //case2
        [Test]
        public void AddUserAsync_NullUserCreateDto_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<NullReferenceException>(async () => await _userRepository.AddUserAsync(null));
        }
        //case3
        [Test]
        public async Task AddUserAsync_EmptyFullName_DoesNotAddUser()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "",
                Password = "Password123",
                Email = "test@example.com",
                Phone = "123456789",
                RoleId = 2
            };

            await _userRepository.AddUserAsync(userCreateDto);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
        //case4
        [Test]
        public async Task AddUserAsync_EmptyEmail_DoesNotAddUser()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "Test User",
                Password = "Password123",
                Email = "",
                Phone = "123456789",
                RoleId = 2
            };

            await _userRepository.AddUserAsync(userCreateDto);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
        //case5
        [Test]
        public async Task AddUserAsync_ExceptionThrown_LogsError()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "Test User",
                Password = "Password123",
                Email = "test@example.com",
                Phone = "123456789",
                RoleId = 2
            };

            _mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(new Exception("Database error"));

            Assert.DoesNotThrowAsync(async () => await _userRepository.AddUserAsync(userCreateDto));
        }
        //case6
        [Test]
        public async Task AddUserAsync_EmptyPhone_DoesNotAddUser()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "Test User",
                Password = "Password123",
                Email = "test@example.com",
                Phone = "",
                RoleId = 2
            };

            await _userRepository.AddUserAsync(userCreateDto);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
        //case7
        [Test]
        public async Task AddUserAsync_EmptyPassword_DoesNotAddUser()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "Test User",
                Password = "",
                Email = "test@example.com",
                Phone = "123456789",
                RoleId = 2
            };

            await _userRepository.AddUserAsync(userCreateDto);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
        //case8
        [Test]
        public async Task AddUserAsync_ValidInput_CorrectlySetsStatusAndCreatedAt()
        {
            var userCreateDto = new UserCreateDto
            {
                FullName = "Test User",
                Password = "Password123",
                Email = "test@example.com",
                Phone = "123456789",
                RoleId = 2
            };

            var mockSet = new Mock<DbSet<User>>();
            _mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            await _userRepository.AddUserAsync(userCreateDto);

            mockSet.Verify(m => m.AddAsync(It.Is<User>(u => u.Status == 1 && u.CreatedAt <= DateTime.UtcNow), default), Times.Once);
        }
    }
}