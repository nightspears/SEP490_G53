using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class VerifyConfirmationCodeTest_ThongND
    {
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        private Sep490G53Context _dbContext;  // Use real DbContext with in-memory provider
        private ICustomerRepository _service;
        private static readonly Dictionary<string, (CustomersAccount CustomersAccount, string Code, DateTime Expiry)> _pendingRegistrations = new();

        [SetUp]
        public void SetUp()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new Sep490G53Context(options);

            _contextAccessorMock = new Mock<IHttpContextAccessor>();

            _service = new CustomerRepository(_dbContext, null, null, _contextAccessorMock.Object);

            // Preload some test data in the in-memory database
            var customerAccount = new CustomersAccount
            {

                Email = "test@example.com",
                ConfirmationCode = "123456",
                CodeExpiry = DateTime.UtcNow.AddMinutes(10),
                Status = 0
            };

            _dbContext.CustomersAccounts.Add(customerAccount);
            _dbContext.SaveChanges();
        }

        [Test]
        public async Task VerifyConfirmationCodeAsync_ValidEmailAndCode_ReturnsTrue()
        {
            // Arrange
            var confirmationCode = "123456";
            var email = "test@example.com";

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, confirmationCode);

            // Assert
            Assert.IsTrue(result);

            // Verify the customer account was updated
            var customer = await _dbContext.CustomersAccounts.FirstOrDefaultAsync(c => c.Email == email);
            Assert.AreEqual(1, customer.Status);  // Customer should now be active (status 1)
            Assert.IsNull(customer.ConfirmationCode);  // Code should be cleared
            Assert.IsNull(customer.CodeExpiry);  // Expiry should be cleared
        }
        [Test]
        public async Task VerifyConfirmationCodeAsync_InvalidCode_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var invalidCode = "invalidcode";
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);

            var customerAccount = new CustomersAccount
            {

                Email = email,
                ConfirmationCode = confirmationCode,
                CodeExpiry = expiry,
                Status = 0
            };

            // Check if the entity is already in the DbContext before adding it.
            if (!_dbContext.CustomersAccounts.Any(ca => ca.CustomerId == customerAccount.CustomerId))
            {
                _dbContext.CustomersAccounts.Add(customerAccount);
                await _dbContext.SaveChangesAsync();
            }

            // Detach the entity to avoid it being tracked by the context
            _dbContext.Entry(customerAccount).State = EntityState.Detached;

            // Query the customer account with AsNoTracking to ensure it's not tracked.
            var trackedCustomerAccount = await _dbContext.CustomersAccounts
                .AsNoTracking()  // This prevents the entity from being tracked.
                .FirstOrDefaultAsync(ca => ca.Email == email);

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, invalidCode);

            // Assert
            Assert.IsFalse(result);
        }





        [Test]
        public async Task VerifyConfirmationCodeAsync_ExpiredCode_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "validcode";
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(-10); // Expired

            var customerAccount = new CustomersAccount
            {

                Email = email,
                ConfirmationCode = confirmationCode,
                CodeExpiry = expiry,
                Status = 0
            };

            // Detach all entities from the DbContext to clear tracking
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }

            // Now add the new entity to the context
            _dbContext.CustomersAccounts.Add(customerAccount);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }


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

        [Test]
        public async Task VerifyConfirmationCodeAsync_NoCodeProvided_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "";
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);

            var customerAccount = new CustomersAccount
            {

                Email = email,
                ConfirmationCode = confirmationCode,
                CodeExpiry = expiry,
                Status = 0
            };

            _dbContext.CustomersAccounts.Add(customerAccount);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task VerifyConfirmationCodeAsync_InvalidCode_CustomerNotAdded()
        {
            // Arrange
            var email = "test@example.com";
            var code = "invalidcode";
            var confirmationCode = "validcode";
            var expiry = DateTime.UtcNow.AddMinutes(10);

            var customerAccount = new CustomersAccount
            {

                Email = email,
                ConfirmationCode = confirmationCode,
                CodeExpiry = expiry,
                Status = 0
            };

            _dbContext.CustomersAccounts.Add(customerAccount);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result);
            var customer = await _dbContext.CustomersAccounts.FindAsync(customerAccount.CustomerId);
            Assert.AreEqual(0, customer.Status);  // Ensure the status is not updated
        }

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

        [Test]
        public async Task VerifyConfirmationCodeAsync_MultipleValidCodes_AllReturnTrue()
        {
            // Arrange
            var email1 = "test1@example.com";
            var email2 = "test2@example.com";
            var code1 = "validcode1";
            var code2 = "validcode2";
            var expiry = DateTime.UtcNow.AddMinutes(10);

            var customer1 = new CustomersAccount { Email = email1, ConfirmationCode = code1, CodeExpiry = expiry, Status = 0 };
            var customer2 = new CustomersAccount { Email = email2, ConfirmationCode = code2, CodeExpiry = expiry, Status = 0 };

            _dbContext.CustomersAccounts.AddRange(customer1, customer2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _service.VerifyConfirmationCodeAsync(email1, code1);
            var result2 = await _service.VerifyConfirmationCodeAsync(email2, code2);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [Test]
        public async Task VerifyConfirmationCodeAsync_MultipleInvalidCodes_AllReturnFalse()
        {
            // Arrange
            var email1 = "test1@example.com";
            var email2 = "test2@example.com";
            var invalidCode1 = "invalidcode1";
            var invalidCode2 = "invalidcode2";
            var expiry = DateTime.UtcNow.AddMinutes(10);

            var customer1 = new CustomersAccount { Email = email1, ConfirmationCode = "validcode1", CodeExpiry = expiry, Status = 0 };
            var customer2 = new CustomersAccount { Email = email2, ConfirmationCode = "validcode2", CodeExpiry = expiry, Status = 0 };

            _dbContext.CustomersAccounts.AddRange(customer1, customer2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _service.VerifyConfirmationCodeAsync(email1, invalidCode1);
            var result2 = await _service.VerifyConfirmationCodeAsync(email2, invalidCode2);

            // Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [Test]
        public async Task VerifyConfirmationCodeAsync_ValidCodeAlreadyUsed_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var code = "validcode";
            var customerAccount = new CustomersAccount
            {

                Email = email,
                ConfirmationCode = code,
                CodeExpiry = DateTime.UtcNow.AddMinutes(10),
                Status = 1 // Assume it was already verified previously
            };

            _dbContext.CustomersAccounts.Add(customerAccount);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.VerifyConfirmationCodeAsync(email, code);

            // Assert
            Assert.IsFalse(result, "The method should not process an already consumed valid code.");
        }





    }
}
