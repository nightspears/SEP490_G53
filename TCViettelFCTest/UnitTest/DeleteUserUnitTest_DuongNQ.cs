using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using System.Threading.Tasks;
using System;

namespace TCViettelFCTest.UnitTest
{
	[TestFixture]
	public class DeleteUserUnitTest_DuongNQ
	{
		private Mock<Sep490G53Context> _contextMock;
		private Mock<IConfiguration> _configurationMock;
		private Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private UserRepository _userRepository;

		[SetUp]
		public void SetUp()
		{
			_contextMock = new Mock<Sep490G53Context>();
			_configurationMock = new Mock<IConfiguration>();
			_httpContextAccessorMock = new Mock<IHttpContextAccessor>();

			_userRepository = new UserRepository(
				_contextMock.Object,
				_configurationMock.Object,
				_httpContextAccessorMock.Object
			);
		}

		[Test]
		public async Task DeleteUserAsync_UserExists_ShouldDeleteUser()
		{
			// Arrange
			var user = new User { UserId = 1, FullName = "TestUser" };
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(user);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act
			await _userRepository.DeleteUserAsync(1);

			// Assert
			dbSetMock.Verify(m => m.Remove(user), Times.Once);
			_contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
		}

		[Test]
		public void DeleteUserAsync_UserDoesNotExist_ShouldThrowKeyNotFoundException()
		{
			// Arrange
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.Setup(m => m.FindAsync(2)).ReturnsAsync((User)null);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act & Assert
			Assert.ThrowsAsync<KeyNotFoundException>(() => _userRepository.DeleteUserAsync(2));
		}

		[Test]
		public async Task DeleteUserAsync_ThrowsExceptionDuringDeletion_ShouldThrowException()
		{
			// Arrange
			var user = new User { UserId = 3, FullName = "TestUser" };
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.Setup(m => m.FindAsync(3)).ReturnsAsync(user);
			dbSetMock.Setup(m => m.Remove(user)).Throws(new Exception("Database error"));
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act & Assert
			var exception = Assert.ThrowsAsync<Exception>(() => _userRepository.DeleteUserAsync(3));
			Assert.AreEqual("Delete failed", exception.Message);
		}

		[Test]
		public async Task DeleteUserAsync_VerifySaveChangesCalledOnce()
		{
			// Arrange
			var user = new User { UserId = 4, FullName = "TestUser" };
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.Setup(m => m.FindAsync(4)).ReturnsAsync(user);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act
			await _userRepository.DeleteUserAsync(4);

			// Assert
			_contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
		}

		[Test]
		public void DeleteUserAsync_NullId_ShouldThrowException()
		{
			// Act & Assert
			Assert.ThrowsAsync<NullReferenceException>(() => _userRepository.DeleteUserAsync(0));
		}

		[Test]
		public void DeleteUserAsync_NegativeId_ShouldThrowException()
		{
			// Act & Assert
			Assert.ThrowsAsync<NullReferenceException>(() => _userRepository.DeleteUserAsync(-1));
		}

		[Test]
		public async Task DeleteUserAsync_DeleteMultipleUsers_ShouldHandleCorrectly()
		{
			// Arrange
			var user1 = new User { UserId = 5, FullName = "User1" };
			var user2 = new User { UserId = 6, FullName = "User2" };
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.SetupSequence(m => m.FindAsync(It.IsAny<int>()))
				.ReturnsAsync(user1)
				.ReturnsAsync(user2);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act
			await _userRepository.DeleteUserAsync(5);
			await _userRepository.DeleteUserAsync(6);

			// Assert
			dbSetMock.Verify(m => m.Remove(user1), Times.Once);
			dbSetMock.Verify(m => m.Remove(user2), Times.Once);
			_contextMock.Verify(c => c.SaveChangesAsync(default), Times.Exactly(2));
		}

		[Test]
		public async Task DeleteUserAsync_UserIsNullAfterDeletion_ShouldHandleGracefully()
		{
			// Arrange
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.Setup(m => m.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act & Assert
			Assert.ThrowsAsync<KeyNotFoundException>(() => _userRepository.DeleteUserAsync(7));
		}

		[Test]
		public async Task DeleteUserAsync_SaveChangesThrowsException_ShouldRethrowException()
		{
			// Arrange
			var user = new User { UserId = 8, FullName = "User8" };
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.Setup(m => m.FindAsync(8)).ReturnsAsync(user);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);
			_contextMock.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(new Exception("Save failed"));

			// Act & Assert
			var exception = Assert.ThrowsAsync<Exception>(() => _userRepository.DeleteUserAsync(8));
			Assert.AreEqual("Save failed", exception.InnerException.Message);
		}

		[Test]
		public async Task DeleteUserAsync_EnsureMethodIsIdempotent()
		{
			// Arrange
			var user = new User { UserId = 9, FullName = "User9" };
			var dbSetMock = new Mock<DbSet<User>>();
			dbSetMock.SetupSequence(m => m.FindAsync(It.IsAny<int>()))
				.ReturnsAsync(user)
				.ReturnsAsync((User)null);
			_contextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

			// Act
			await _userRepository.DeleteUserAsync(9);

			// Assert
			Assert.ThrowsAsync<KeyNotFoundException>(() => _userRepository.DeleteUserAsync(9));
		}
	}
}
