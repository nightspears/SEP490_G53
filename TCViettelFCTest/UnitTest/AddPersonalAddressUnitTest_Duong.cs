using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Models;
using TCViettelFC_API.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Http;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
	[TestFixture]
	public class AddPersonalAddressUnitTest_Duong
	{
		private Mock<Sep490G53Context> _mockContext;
		private Mock<DbSet<PersonalAddress>> _mockDbSet;
		private Mock<IConfiguration> _mockConfiguration;
		private Mock<IEmailService> _mockEmailService;
		private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
		private CustomerRepository _repository;

		[SetUp]
		public void Setup()
		{
			_mockContext = new Mock<Sep490G53Context>();
			_mockDbSet = new Mock<DbSet<PersonalAddress>>();
			_mockConfiguration = new Mock<IConfiguration>();
			_mockEmailService = new Mock<IEmailService>();
			_mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

			_mockContext.Setup(c => c.PersonalAddresses).Returns(_mockDbSet.Object);

			_repository = new CustomerRepository(
				_mockContext.Object,
				_mockConfiguration.Object,
				_mockEmailService.Object,
				_mockHttpContextAccessor.Object
			);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_ValidInput_ReturnsTrue()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsTrue(result);
			_mockDbSet.Verify(db => db.AddAsync(It.IsAny<PersonalAddress>(), default), Times.Once);
			_mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_NullInput_ReturnsFalse()
		{
			var result = await _repository.InsertPersonalAddressAsync(null);

			Assert.IsFalse(result);
			_mockDbSet.Verify(db => db.AddAsync(It.IsAny<PersonalAddress>(), default), Times.Never);
			_mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_EmptyDetailedAddress_ReturnsTrue()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "",
				Status = 0
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsTrue(result);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_NullCityName_ReturnsTrue()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = null,
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsTrue(result);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_InvalidCustomerId_ReturnsFalse()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 0, // Invalid ID
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsTrue(result);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_DatabaseException_ReturnsFalse()
		{
			_mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(new Exception("Database error"));

			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsFalse(result);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_DuplicateAddress_IsAccepted()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			// Act
			var result = await _repository.InsertPersonalAddressAsync(dto);

			// Assert
			Assert.IsTrue(result);
			_mockDbSet.Verify(db => db.AddAsync(It.IsAny<PersonalAddress>(), default), Times.Once);
			_mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
		}


		[Test]
		public async Task InsertPersonalAddressAsync_InvalidStatus_ReturnsTrue()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = -1 // Edge case for status
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsTrue(result);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_ValidInput_SaveChangesCalledOnce()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			await _repository.InsertPersonalAddressAsync(dto);

			_mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_ValidInput_DbSetAddAsyncCalledOnce()
		{
			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			await _repository.InsertPersonalAddressAsync(dto);

			_mockDbSet.Verify(db => db.AddAsync(It.IsAny<PersonalAddress>(), default), Times.Once);
		}

		[Test]
		public async Task InsertPersonalAddressAsync_ExceptionThrown_ReturnsFalse()
		{
			_mockDbSet.Setup(db => db.AddAsync(It.IsAny<PersonalAddress>(), default))
					  .ThrowsAsync(new Exception("Unexpected error"));

			var dto = new PersonalAddressCreateDto
			{
				CustomerId = 1,
				CityName = "Hanoi",
				City = "HN",
				DistrictName = "Ba Dinh",
				District = "BD",
				WardName = "Truc Bach",
				Ward = "TB",
				DetailedAddress = "123 Street",
				Status = 0
			};

			var result = await _repository.InsertPersonalAddressAsync(dto);

			Assert.IsFalse(result);
		}
	}
}




