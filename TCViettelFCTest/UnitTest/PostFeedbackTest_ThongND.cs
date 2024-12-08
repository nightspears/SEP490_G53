using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Security.Claims;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class PostFeedbackTest_ThongND
    {
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        private Mock<Sep490G53Context> _dbContextMock;
        private ICustomerRepository _service;

        [SetUp]
        public void Setup()
        {
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _dbContextMock = new Mock<Sep490G53Context>();
            _service = new CustomerRepository(_dbContextMock.Object, null, null, _contextAccessorMock.Object);
        }

        [Test]
        public async Task PostFeedback_ValidFeedback_Returns1()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Great service!" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "1") });

            _dbContextMock.Setup(x => x.Customers.AddAsync(It.IsAny<Customer>(), default))
    .ReturnsAsync((EntityEntry<Customer>)null!);

            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .ReturnsAsync((EntityEntry<Feedback>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task PostFeedback_NoCustomerIdInClaims_Returns0()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Great service!" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(Enumerable.Empty<Claim>());
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task PostFeedback_CustomerIdInvalid_Returns0()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Great service!" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "invalid") });
            Assert.ThrowsAsync<FormatException>(() => _service.PostFeedback(feedbackDto));
        }

        [Test]
        public async Task PostFeedback_SaveChangesThrowsException_Returns0()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Great service!" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "1") });

            _dbContextMock.Setup(x => x.Customers.AddAsync(It.IsAny<Customer>(), default))
    .ReturnsAsync((EntityEntry<Customer>)null!);

            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .ReturnsAsync((EntityEntry<Feedback>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .Throws(new Exception());
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task PostFeedback_NullFeedbackDto_Returns0()
        {
            var result = await _service.PostFeedback(null);
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task PostFeedback_EmptyContent_Returns1()
        {
            var feedbackDto = new FeedbackPostDto { Content = "" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "1") });

            _dbContextMock.Setup(x => x.Customers.AddAsync(It.IsAny<Customer>(), default))
    .ReturnsAsync((EntityEntry<Customer>)null!);

            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .ReturnsAsync((EntityEntry<Feedback>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task PostFeedback_NullContent_Returns1()
        {
            var feedbackDto = new FeedbackPostDto { Content = null };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "1") });

            _dbContextMock.Setup(x => x.Customers.AddAsync(It.IsAny<Customer>(), default))
     .ReturnsAsync((EntityEntry<Customer>)null!);

            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .ReturnsAsync((EntityEntry<Feedback>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task PostFeedback_DatabaseSaveFails_Returns0()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Feedback content" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "1") });

            _dbContextMock.Setup(x => x.Customers.AddAsync(It.IsAny<Customer>(), default))
    .ReturnsAsync((EntityEntry<Customer>)null!);

            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .ReturnsAsync((EntityEntry<Feedback>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(0); // Simulate database save failure
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task PostFeedback_ValidCustomerAndFeedback_SavesCustomerAndFeedback()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Feedback content" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "2") });

            var customerAdded = false;
            var feedbackAdded = false;

            _dbContextMock.Setup(x => x.Customers.AddAsync(It.IsAny<Customer>(), default))
                .Callback<Customer, CancellationToken>((customer, _) => customerAdded = true)
                .ReturnsAsync((EntityEntry<Customer>)null!);
            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .Callback<Feedback, CancellationToken>((feedback, _) => feedbackAdded = true)
                .ReturnsAsync((EntityEntry<Feedback>)null!);
            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(1, result, "The PostFeedback method should return 1 for successful feedback submission.");
            Assert.IsTrue(customerAdded, "Customer should be added to the database.");
            Assert.IsTrue(feedbackAdded, "Feedback should be added to the database.");
        }


        [Test]
        public async Task PostFeedback_ValidData_FeedbackStatusIs0()
        {
            var feedbackDto = new FeedbackPostDto { Content = "Test feedback" };
            _contextAccessorMock.Setup(x => x.HttpContext!.User.Claims)
                .Returns(new[] { new Claim("CustomerId", "3") });

            Feedback addedFeedback = null;
            _dbContextMock.Setup(x => x.Feedbacks.AddAsync(It.IsAny<Feedback>(), default))
                .Callback<Feedback, CancellationToken>((feedback, _) => addedFeedback = feedback)
                .ReturnsAsync((EntityEntry<Feedback>)null!);

            _dbContextMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            var result = await _service.PostFeedback(feedbackDto);
            Assert.AreEqual(1, result);
            Assert.IsNotNull(addedFeedback);
            Assert.AreEqual(0, addedFeedback.Status);
        }

    }
}
