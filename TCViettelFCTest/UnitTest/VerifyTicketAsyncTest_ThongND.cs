using Microsoft.AspNetCore.Mvc;
using Moq;
using TCViettelFC_API.Controllers;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class VerifyTicketAsyncTest_ThongND
    {
        private Mock<ITicketUtilRepository> _ticketUtilRepositoryMock;
        private EntryController _controller;

        [SetUp]
        public void Setup()
        {
            _ticketUtilRepositoryMock = new Mock<ITicketUtilRepository>();
            _controller = new EntryController(_ticketUtilRepositoryMock.Object);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketExistsAndValid_ReturnsOkResult()
        {
            // Arrange
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, Price = 100 }; // Valid status
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(1); // Ticket is valid

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Ticket verified successfully", okResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int ticketId = 1;
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync((OrderedTicket)null);

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Ticket not found", notFoundResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketStatusInvalid_ReturnsBadRequestResult()
        {
            // Arrange
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 2, Price = 100 };
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(0);

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketHasNoPrice_ReturnsBadRequestResult()
        {
            // Arrange
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, Price = null };
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(1);

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketWithInvalidMatch_ReturnsBadRequest()
        {
            // Arrange
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, MatchId = null }; // No match for ticket
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(0); // Invalid ticket due to no match

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketWithValidMatchAndArea_ReturnsOk()
        {
            // Arrange
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, MatchId = 123, AreaId = 456, Price = 100 }; // Valid match and area
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(1); // Valid ticket

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Ticket verified successfully", okResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketAlreadyVerified_ReturnsBadRequest()
        {
            // Arrange
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, Price = 100 }; // Already verified (status 2)
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(0); // Already verified, shouldn't be valid for re-verification

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketWithNegativePrice_ReturnsBadRequest()
        {
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, Price = -10 }; // Negative price

            // Mock the GetOrderedTicketByIdAsync method to return the ticket with negative price
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);

            // Act
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }


        [Test]
        public async Task VerifyTicketAsync_TicketWithMissingArea_ReturnsBadRequest()
        {
            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = 1, AreaId = null, Price = 100 };
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);
            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(0);
            var result = await _controller.VerifyTicketAsync(ticketId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }

        [Test]
        public async Task VerifyTicketAsync_TicketWithNullStatus_ReturnsBadRequest()
        {

            int ticketId = 1;
            var orderedTicket = new OrderedTicket { Id = ticketId, Status = null, Price = 100 }; // Null status
            _ticketUtilRepositoryMock.Setup(repo => repo.GetOrderedTicketByIdAsync(ticketId)).ReturnsAsync(orderedTicket);

            _ticketUtilRepositoryMock.Setup(repo => repo.VerifyTicketAsync(orderedTicket)).ReturnsAsync(0);


            var result = await _controller.VerifyTicketAsync(ticketId);


            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Ticket not valid", badRequestResult.Value);
        }

    }
}
