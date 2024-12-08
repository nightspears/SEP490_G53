using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class VerifyConfirmationCodeTest_ThongND
    {
        private Mock<Sep490G53Context> _dbContextMock;
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        private ICustomerRepository _service;
        private static readonly Dictionary<string, (CustomersAccount CustomersAccount, string Code, DateTime Expiry)> _pendingRegistrations = new();


        [SetUp]
        public void SetUp()
        {
            _dbContextMock = new Mock<Sep490G53Context>();

            _contextAccessorMock = new Mock<IHttpContextAccessor>();


            _service = new CustomerRepository(_dbContextMock.Object, null, null, _contextAccessorMock.Object);
        }

        // Test Case 1: Valid Email and Code
        [Test]
        public async Task VerifyConfirmationCodeAsync_ValidEmailAndCode_ReturnsTrue()
        {
            // Arrange
            var CustomersAccount = new CustomersAccount
            {
                CustomerId = 1,
                Password = "1234",
                Email = "test@example.com",
            };

            var confirmationCode = "123456";
            var expiry = DateTime.UtcNow.AddMinutes(10);
            _pendingRegistrations["test@example.com"] = (CustomersAccount, confirmationCode, expiry);

            _dbContextMock.Setup(db => db.CustomersAccounts.AddAsync(It.IsAny<CustomersAccount>(), It.IsAny<CancellationToken>()))
     .ReturnsAsync((CustomersAccount account, CancellationToken _) =>
         Mock.Of<EntityEntry<CustomersAccount>>(e => e.Entity == account));

            _dbContextMock.Setup(db => db.Profiles.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Profile account, CancellationToken _) =>
         Mock.Of<EntityEntry<Profile>>(e => e.Entity == account));
            _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.VerifyConfirmationCodeAsync("test@example.com", confirmationCode);

            // Assert
            Assert.IsTrue(result);
            _dbContextMock.Verify(db => db.CustomersAccounts.AddAsync(It.Is<CustomersAccount>(c => c.CustomerId == CustomersAccount.CustomerId), default), Times.Once);
            _dbContextMock.Verify(db => db.Profiles.AddAsync(It.Is<Profile>(p => p.CustomerId == CustomersAccount.CustomerId), default), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Exactly(2));
            Assert.IsFalse(_pendingRegistrations.ContainsKey("test@example.com"));
        }




        // Test Case 2: Invalid Code
        [Test]
        public async Task VerifyConfirmationCodeAsync_InvalidCode_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "invalidcode";
            var CustomersAccount = new CustomersAccount { CustomerId = 1 };
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);
            _pendingRegistrations[email] = (CustomersAccount, confirmationCode, expiry);

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 3: Expired Code
        [Test]
        public async Task VerifyConfirmationCodeAsync_ExpiredCode_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "validcode";
            var CustomersAccount = new CustomersAccount { CustomerId = 1 };
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(-10); // Expired
            _pendingRegistrations[email] = (CustomersAccount, confirmationCode, expiry);

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 4: Email Not Found
        [Test]
        public async Task VerifyConfirmationCodeAsync_EmailNotFound_ReturnsFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var code = "validcode";

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 5: No Code Provided
        [Test]
        public async Task VerifyConfirmationCodeAsync_NoCodeProvided_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "";
            var CustomersAccount = new CustomersAccount { CustomerId = 1 };
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);
            _pendingRegistrations[email] = (CustomersAccount, confirmationCode, expiry);

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 6: CustomersAccount is Not Added if Code is Invalid
        [Test]
        public async Task VerifyConfirmationCodeAsync_InvalidCode_CustomerNotAdded()
        {
            // Arrange
            var email = "test@example.com";
            var code = "invalidcode";
            var CustomersAccount = new CustomersAccount { CustomerId = 1 };
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);
            _pendingRegistrations[email] = (CustomersAccount, confirmationCode, expiry);
            _dbContextMock.Setup(x => x.CustomersAccounts.AddAsync(It.IsAny<CustomersAccount>(), default))
              .ReturnsAsync((EntityEntry<CustomersAccount>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
            _dbContextMock.Verify(x => x.CustomersAccounts.AddAsync(It.IsAny<CustomersAccount>(), default), Times.Never);
        }



        // Test Case 8: No Pending Registration for the Email
        [Test]
        public async Task VerifyConfirmationCodeAsync_NoPendingRegistration_NoAction()
        {
            // Arrange
            var email = "unregistered@example.com";
            var code = "validcode";

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }

        // Test Case 9: Multiple Valid Codes
        [Test]
        public async Task VerifyConfirmationCodeAsync_MultipleValidCodes_AllReturnTrue()
        {
            // Arrange
            var email1 = "test1@example.com";
            var email2 = "test2@example.com";
            var code1 = "validcode1";
            var code2 = "validcode2";
            var customer1 = new CustomersAccount { CustomerId = 1 };
            var customer2 = new CustomersAccount { CustomerId = 2 };
            var expiry = DateTime.UtcNow.AddMinutes(10);

            _pendingRegistrations[email1] = (customer1, "validcode1", expiry);
            _pendingRegistrations[email2] = (customer2, "validcode2", expiry);

            _dbContextMock.Setup(x => x.CustomersAccounts.AddAsync(It.IsAny<CustomersAccount>(), default))
              .ReturnsAsync((EntityEntry<CustomersAccount>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result1 = await _service.VerifyConfirmationCodeAsync(email1, code1);
            var result2 = await _service.VerifyConfirmationCodeAsync(email2, code2);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        // Test Case 10: Multiple Invalid Codes
        [Test]
        public async Task VerifyConfirmationCodeAsync_MultipleInvalidCodes_AllReturnFalse()
        {
            // Arrange
            var email1 = "test1@example.com";
            var email2 = "test2@example.com";
            var invalidCode1 = "invalidcode1";
            var invalidCode2 = "invalidcode2";
            var customer1 = new CustomersAccount { CustomerId = 1 };
            var customer2 = new CustomersAccount { CustomerId = 2 };
            var expiry = DateTime.UtcNow.AddMinutes(10);

            _pendingRegistrations[email1] = (customer1, "validcode1", expiry);
            _pendingRegistrations[email2] = (customer2, "validcode2", expiry);

            // Act
            var result1 = await _service.VerifyConfirmationCodeAsync(email1, invalidCode1);
            var result2 = await _service.VerifyConfirmationCodeAsync(email2, invalidCode2);

            // Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
        // Test Case 11: Valid Code Already Used
        [Test]
        public async Task VerifyConfirmationCodeAsync_ValidCodeAlreadyUsed_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "validcode";
            var customerAccount = new CustomersAccount { CustomerId = 1 };
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);

            // Adding an already consumed registration (simulated by not removing it after previous processing)
            _pendingRegistrations[email] = (customerAccount, confirmationCode, expiry);

            // Simulate that the account has already been added previously and saved to the database
            _dbContextMock.Setup(db => db.CustomersAccounts.AddAsync(It.IsAny<CustomersAccount>(), default))
                .ReturnsAsync((EntityEntry<CustomersAccount>)null!); // Mock that no new entity is being added
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1); // Simulate save to database

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result, "The method should not process an already consumed valid code.");
            _dbContextMock.Verify(db => db.CustomersAccounts.AddAsync(It.IsAny<CustomersAccount>(), default), Times.Never);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
        }

    }
}
