using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;

namespace TCViettelFCTest.UnitTest
{
	[TestFixture]
	public class UpdateUserUnitTest_Duong
	{
		private Mock<Sep490G53Context> _mockContext;
		private Mock<DbSet<User>> _mockDbSet;
		private UserRepository _userRepository;

		[SetUp]
		public void Setup()
		{
			_mockContext = new Mock<Sep490G53Context>();
			_mockDbSet = new Mock<DbSet<User>>();
			_mockContext.Setup(m => m.Users).Returns(_mockDbSet.Object);

			var mockConfiguration = new Mock<IConfiguration>();
			var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

			_userRepository = new UserRepository(_mockContext.Object, mockConfiguration.Object, mockHttpContextAccessor.Object);
		}

		[Test]
		public async Task UpdateUserAsync_ShouldUpdateAllFields_WhenAllFieldsProvided()
		{
			var user = new User { UserId = 1, Email = "old@example.com", Phone = "123456789", FullName = "Old Name", RoleId = 2, Status = 1, CreatedAt = DateTime.Now };
			_mockDbSet.Setup(m => m.FindAsync(1)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { Email = "new@example.com", Phone = "987654321", FullName = "New Name", RoleId = 3, Status = 2, CreatedAt = DateTime.Now.AddDays(-1) };

			await _userRepository.UpdateUserAsync(1, userDto);

			Assert.AreEqual("new@example.com", user.Email);
			Assert.AreEqual("987654321", user.Phone);
			Assert.AreEqual("New Name", user.FullName);
			Assert.AreEqual(3, user.RoleId);
			Assert.AreEqual(2, user.Status);
			Assert.AreEqual(userDto.CreatedAt, user.CreatedAt);
		}

		[Test]
		public async Task UpdateUserAsync_ShouldUpdateSomeFields_WhenPartialFieldsProvided()
		{
			var user = new User { UserId = 2, Email = "old@example.com", Phone = "123456789", FullName = "Old Name", RoleId = 2, Status = 1, CreatedAt = DateTime.Now };
			_mockDbSet.Setup(m => m.FindAsync(2)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { Email = null, Phone = "987654321", FullName = null, RoleId = 3 };

			await _userRepository.UpdateUserAsync(2, userDto);

			Assert.AreEqual("old@example.com", user.Email);
			Assert.AreEqual("987654321", user.Phone);
			Assert.AreEqual("Old Name", user.FullName);
			Assert.AreEqual(3, user.RoleId);
			Assert.AreEqual(1, user.Status);
		}

		[Test]
		public void UpdateUserAsync_ShouldThrowException_WhenUserNotFound()
		{
			_mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null);

			var userDto = new UserUpdateDto { Email = "new@example.com" };

			Assert.ThrowsAsync<Exception>(async () => await _userRepository.UpdateUserAsync(3, userDto), "User not found");
		}

		[Test]
		public void UpdateUserAsync_ShouldThrowException_WhenDatabaseSaveFails()
		{
			var user = new User { UserId = 4 };
			_mockDbSet.Setup(m => m.FindAsync(4)).ReturnsAsync(user);
			_mockContext.Setup(m => m.SaveChangesAsync(default)).Throws(new Exception("Database error"));

			var userDto = new UserUpdateDto { Email = "new@example.com" };

			Assert.ThrowsAsync<Exception>(async () => await _userRepository.UpdateUserAsync(4, userDto), "Error updating user: Database error");
		}

		[Test]
		public async Task UpdateUserAsync_ShouldNotChangeUnprovidedFields()
		{
			var user = new User { UserId = 5, Email = "old@example.com", Phone = "123456789" };
			_mockDbSet.Setup(m => m.FindAsync(5)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { Email = "new@example.com" };

			await _userRepository.UpdateUserAsync(5, userDto);

			Assert.AreEqual("new@example.com", user.Email);
			Assert.AreEqual("123456789", user.Phone); // Phone should remain unchanged
		}

		[Test]
		public void UpdateUserAsync_ShouldThrowException_WhenUserIdIsInvalid()
		{
			var userDto = new UserUpdateDto { Email = "new@example.com" };

			Assert.ThrowsAsync<Exception>(async () => await _userRepository.UpdateUserAsync(-1, userDto), "User not found");
		}

		[Test]
		public void UpdateUserAsync_ShouldThrowException_WhenRoleIdIsInvalid()
		{
			var user = new User { UserId = 6, RoleId = 1, Status = 1 };
			_mockDbSet.Setup(m => m.FindAsync(6)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { RoleId = -1 }; // Invalid RoleId

			var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _userRepository.UpdateUserAsync(6, userDto));
			Assert.AreEqual("Invalid RoleId.", exception.Message);
		}

		[Test]
		public async Task UpdateUserAsync_ShouldAllowEmptyStrings_ForEmailAndPhone()
		{
			var user = new User { UserId = 7, Email = "old@example.com", Phone = "123456789" };
			_mockDbSet.Setup(m => m.FindAsync(7)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { Email = "", Phone = "" };

			await _userRepository.UpdateUserAsync(7, userDto);

			Assert.AreEqual("", user.Email);
			Assert.AreEqual("", user.Phone);
		}

		[Test]
		public async Task UpdateUserAsync_ShouldHandleNullOptionalFields()
		{
			var user = new User { UserId = 8, CreatedAt = DateTime.Now };
			_mockDbSet.Setup(m => m.FindAsync(8)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { CreatedAt = null };

			await _userRepository.UpdateUserAsync(8, userDto);

			Assert.AreEqual(user.CreatedAt, user.CreatedAt); // Should remain unchanged
		}

		[Test]
		public async Task UpdateUserAsync_ShouldNotChangeCreatedAt_WhenNotProvided()
		{
			var originalCreatedAt = DateTime.Now;
			var user = new User { UserId = 9, CreatedAt = originalCreatedAt };
			_mockDbSet.Setup(m => m.FindAsync(9)).ReturnsAsync(user);

			var userDto = new UserUpdateDto { };

			await _userRepository.UpdateUserAsync(9, userDto);

			Assert.AreEqual(originalCreatedAt, user.CreatedAt); // Should remain unchanged
		}
	}
}
