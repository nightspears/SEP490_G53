using Microsoft.EntityFrameworkCore;
using Moq;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
    public static class AsyncEnumerableExtensions
    {
        public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> list) where T : class
        {
            var queryable = list.AsQueryable();

            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbSetMock.As<IAsyncEnumerable<T>>().Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<T>(queryable.GetEnumerator()));

            return dbSetMock;
        }

        private class AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public AsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }

            public T Current => _enumerator.Current;
        }
    }

    [TestFixture]
    public class OrderRepositoryTest_ThongND
    {
        private Mock<Sep490G53Context> _contextMock;
        private IOrderRepository _repository;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<Sep490G53Context>();
            _repository = new OrderRepository(_contextMock.Object);
        }

        [Test]
        public async Task GetAllTicketOrders_NoTicketOrders_ReturnsEmptyList()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>();
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllTicketOrders_SingleOrderWithNullCustomer_ReturnsCorrectData()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
            {
                new TicketOrder { Id = 1, OrderDate = new DateTime(2024, 11, 1), TotalAmount = 100, Customer = null }
            };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsNull(result.First().CustomerEmail);
        }

        [Test]
        public async Task GetAllTicketOrders_MultipleOrdersWithMixedCustomers_ReturnsCorrectData()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
            {
                new TicketOrder { Id = 1, OrderDate = new DateTime(2024, 11, 1), TotalAmount = 100, Customer = null },
                new TicketOrder { Id = 2, OrderDate = new DateTime(2024, 11, 2), TotalAmount = 150, Customer = new Customer { Email = "test@example.com" } }
            };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsNull(result.ElementAt(0).CustomerEmail);
            Assert.AreEqual("test@example.com", result.ElementAt(1).CustomerEmail);
        }

        [Test]
        public async Task GetAllTicketOrders_MultipleOrders_VerifyOrderDetails()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
            {
                new TicketOrder { Id = 1, OrderDate = new DateTime(2024, 11, 1), TotalAmount = 100 },
                new TicketOrder { Id = 2, OrderDate = new DateTime(2024, 11, 2), TotalAmount = 150 }
            };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(100, result.ElementAt(0).TotalAmount);
            Assert.AreEqual(150, result.ElementAt(1).TotalAmount);
        }

        [Test]
        public async Task GetAllTicketOrders_OrderWithNullTotalAmount_ReturnException()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
    {
        new TicketOrder { Id = 1, OrderDate = new DateTime(2024, 11, 1), TotalAmount = null, Customer = new Customer { Email = "test@example.com" } }
    };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(_repository.GetAllTicketOrders);
        }




        [Test]
        public void GetAllTicketOrders_DatabaseFailure_ThrowsException()
        {
            // Arrange
            _contextMock.Setup(c => c.TicketOrders).Throws(new Exception("Database failure"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(_repository.GetAllTicketOrders);
        }

        [Test]
        public async Task GetAllTicketOrders_OrderWithNullEmailInCustomer()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
            {
                new TicketOrder { Id = 1, Customer = new Customer { Email = null } }
            };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsNull(result.First().CustomerEmail);
        }

        [Test]
        public async Task GetAllTicketOrders_LargeNumberOfOrders()
        {
            // Arrange
            var ticketOrders = Enumerable.Range(1, 1000).Select(i => new TicketOrder
            {
                Id = i,
                OrderDate = DateTime.Now,
                TotalAmount = 100 + i,
                Customer = new Customer { Email = "abc" + i }

            }).ToList();
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.AreEqual(1000, result.Count());
        }
        [Test]
        public async Task GetAllTicketOrders_ReturnsAllOrders()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
    {
        new TicketOrder
        {
            Id = 1,
            OrderDate = new DateTime(2024, 11, 1),
            TotalAmount = 150,
            Customer = new Customer { Email = "customer1@example.com" }
        },
        new TicketOrder
        {
            Id = 2,
            OrderDate = new DateTime(2024, 11, 2),
            TotalAmount = 200,
            Customer = new Customer { Email = "customer2@example.com" }
        }
    };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTicketOrders();

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsNotNull(result.FirstOrDefault(r => r.CustomerEmail == "customer1@example.com"));
            Assert.IsNotNull(result.FirstOrDefault(r => r.CustomerEmail == "customer2@example.com"));
            Assert.IsNull(result.FirstOrDefault(r => r.Id == 3)?.CustomerEmail);
            Assert.AreEqual(150, result.FirstOrDefault(r => r.Id == 1)?.TotalAmount);
        }
        [Test]
        public async Task GetAllTicketOrders_InvalidOrExtremeDates_ReturnsValidResults()
        {
            // Arrange
            var ticketOrders = new List<TicketOrder>
    {
        new TicketOrder
        {
            Id = 1,
            OrderDate = DateTime.MinValue,
            TotalAmount = 150,
            Customer = new Customer { Email = "customer1@example.com" }
        },
        new TicketOrder
        {
            Id = 2,
            OrderDate = DateTime.MaxValue,
            TotalAmount = 200,
            Customer = new Customer { Email = "customer2@example.com" }
        },
        new TicketOrder
        {
            Id = 3,
            OrderDate = new DateTime(2024, 11, 1),
            TotalAmount = 100,
            Customer = new Customer { Email = "customer3@example.com" }
        }
    };
            var mockSet = AsyncEnumerableExtensions.CreateMockDbSet(ticketOrders);
            _contextMock.Setup(c => c.TicketOrders).Returns(mockSet.Object);
            var result = await _repository.GetAllTicketOrders();
            Assert.AreEqual(3, result.Count());

            Assert.AreEqual(DateTime.MinValue, result.FirstOrDefault(r => r.Id == 1)?.OrderDate);
            Assert.AreEqual(DateTime.MaxValue, result.FirstOrDefault(r => r.Id == 2)?.OrderDate);
            Assert.AreEqual(new DateTime(2024, 11, 1), result.FirstOrDefault(r => r.Id == 3)?.OrderDate);
        }



    }
}
