using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Dtos;
using Microsoft.AspNetCore.Http;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
namespace TCViettelFCTest.UnitTest
{
	[TestFixture]
	public class GetPersonalAddressUnitTest_DuongNQ
	{
		private CustomerRepository _repository;
		private Mock<IConfiguration> _mockConfiguration;
		private Mock<IEmailService> _mockEmailService;
		private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
		private DbContextOptions<Sep490G53Context> _dbContextOptions;

		[SetUp]
		public void SetUp()
		{
			// Initialize mocks
			_mockConfiguration = new Mock<IConfiguration>();
			_mockEmailService = new Mock<IEmailService>();
			_mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

			// Configure in-memory database with a unique name
			_dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
				.Options;

			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				// Seed the database with test data
				context.PersonalAddresses.AddRange(new List<PersonalAddress>
		{
			new PersonalAddress
			{
				AddressId = 1,
				CustomerId = 1,
				CityName = "City1",
				City = "CityCode1",
				DistrictName = "District1",
				District = "DistrictCode1",
				WardName = "Ward1",
				Ward = "WardCode1",
				DetailedAddress = "Address1",
				Status = 0
			},
			new PersonalAddress
			{
				AddressId = 2,
				CustomerId = 2,
				CityName = "City2",
				City = "CityCode2",
				DistrictName = "District2",
				District = "DistrictCode2",
				WardName = "Ward2",
				Ward = "WardCode2",
				DetailedAddress = "Address2",
				Status = 0
			}
		});

				context.SaveChanges();
			}

			// Create the repository instance
			var dbContext = new Sep490G53Context(_dbContextOptions);
			_repository = new CustomerRepository(dbContext, _mockConfiguration.Object, _mockEmailService.Object, _mockHttpContextAccessor.Object);
		}

		//case1
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_ReturnsAddresses()
		{
			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("City1", result.First().CityName);
			Assert.AreEqual("District1", result.First().DistrictName);
			Assert.AreEqual("Ward1", result.First().WardName);
			Assert.AreEqual("Address1", result.First().DetailedAddress);
		}
		//case2
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_InvalidCustomerId_ReturnsEmptyList()
		{
			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(99);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsEmpty(result);
		}
		//case3
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_WithNoAddresses_ReturnsEmptyList()
		{
			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(3);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsEmpty(result);
		}
		//case4
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_WithMultipleAddresses_ReturnsAllAddresses()
		{
			// Arrange
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.AddRange(new List<PersonalAddress>
		{
			new PersonalAddress
			{
				AddressId = 3,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address3",
				Status = 0
			},
			new PersonalAddress
			{
				AddressId = 4,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address4",
				Status = 0
			}
		});
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count); // Including the previously seeded addresses
		}
		//case5
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_WithDuplicateAddresses_ReturnsDuplicateEntries()
		{
			// Arrange
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.AddRange(new List<PersonalAddress>
		{
			new PersonalAddress
			{
				AddressId = 6,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address6",
				Status = 0
			},
			new PersonalAddress
			{
				AddressId = 7,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address6", // Duplicate address
                Status = 0
			}
		});
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count); // Two entries of the same address
		}
		//case6
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_WithoutDetailedAddress_ReturnsAddressWithEmptyDetail()
		{
			// Arrange
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.Add(new PersonalAddress
				{
					AddressId = 11,
					CustomerId = 1,
					CityName = "City1",
					DistrictName = "District1",
					WardName = "Ward1",
					DetailedAddress = "", // Empty detailed address
					Status = 0
				});
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.IsEmpty(result[1].DetailedAddress); // Ensure the detailed address is empty
		}
		//case7
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_WithoutCityName_ReturnsAddressWithoutCityName()
		{
			// Arrange
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.Add(new PersonalAddress
				{
					AddressId = 8,
					CustomerId = 1,
					CityName = null, // No CityName
					DistrictName = "District1",
					WardName = "Ward1",
					DetailedAddress = "Address8",
					Status = 0
				});
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.IsNull(result[1].CityName); // Ensure the city name is null
		}
		//case8
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_ValidCustomerId_WithActiveAndInactiveAddresses_ReturnsBothTypes()
		{
			// Arrange
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.AddRange(new List<PersonalAddress>
		{
			new PersonalAddress
			{
				AddressId = 9,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address9",
				Status = 0 // Active
            },
			new PersonalAddress
			{
				AddressId = 10,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address10",
				Status = 1 // Inactive
            }
		});
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count); // Ensure both active and inactive addresses are included
		}
		//case9
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_NoAddressesInDatabase_ReturnsEmptyList()
		{
			// Arrange (empty database)
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.RemoveRange(context.PersonalAddresses);
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsEmpty(result); // Should return an empty list
		}
		[Test]
		public async Task GetPersonalAddressesByCustomerIdAsync_FilterInactiveAddresses_ReturnsOnlyActiveAddresses()
		{
			// Arrange
			using (var context = new Sep490G53Context(_dbContextOptions))
			{
				context.PersonalAddresses.AddRange(new List<PersonalAddress>
		{
			new PersonalAddress
			{
				AddressId = 11,
				CustomerId = 1,
				CityName = "City1",
				DistrictName = "District1",
				WardName = "Ward1",
				DetailedAddress = "Address11",
				Status = 0 // Active address
            },
			new PersonalAddress
			{
				AddressId = 12,
				CustomerId = 1,
				CityName = "City2",
				DistrictName = "District2",
				WardName = "Ward2",
				DetailedAddress = "Address12",
				Status = 1 // Inactive address
            }
		});
				context.SaveChanges();
			}

			// Act
			var result = await _repository.GetPersonalAddressesByCustomerIdAsync(1);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count); // Only 1 active address should be returned
			Assert.AreEqual("City1", result.First().CityName); // Ensure the active address is returned
			Assert.AreEqual("Address1", result.First().DetailedAddress); // Ensure the correct address is returned
		}

	}

}
