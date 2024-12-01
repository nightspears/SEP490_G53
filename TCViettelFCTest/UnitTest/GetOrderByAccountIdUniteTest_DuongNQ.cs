using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Models;
using TCViettelFC_API.Dtos.Order;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace TCViettelFCTest.UnitTest
{
	[TestFixture]
	public class GetOrderByAccountIdUnitTest_DuongNQ
	{
		private Mock<Sep490G53Context> _mockContext;
		private OrderRepository _orderRepository;

		[SetUp]
		public void SetUp()
		{
			_mockContext = new Mock<Sep490G53Context>();
			_orderRepository = new OrderRepository(_mockContext.Object);
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_ValidCustomerId_ReturnsOrders()
		{
			// Arrange
			var customerId = 1;
			var orders = new List<OrderProduct>
			{
				new OrderProduct { Id = 1, OrderCode = "ORD001", TotalPrice = 100.5m, Customer = new Customer { AccountId = customerId } },
				new OrderProduct { Id = 2, OrderCode = "ORD002", TotalPrice = 200.0m, Customer = new Customer { AccountId = customerId } }
			}.AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.AreEqual(2, result.Count());
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_NoOrdersForCustomer_ReturnsEmptyList()
		{
			// Arrange
			var customerId = 1;
			var orders = new List<OrderProduct>().AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.IsEmpty(result);
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_NullCustomerId_ThrowsException()
		{
			// Arrange
			var customerId = -1;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId));
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_IncludeCustomerDetails_ReturnsOrdersWithCustomer()
		{
			// Arrange
			var customerId = 1;
			var orders = new List<OrderProduct>
			{
				new OrderProduct { Id = 1, Customer = new Customer { AccountId = customerId, FullName = "John Doe" } }
			}.AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.AreEqual(1, result.Count());
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_IncludeShipmentDetails_ReturnsOrdersWithShipmentFee()
		{
			// Arrange
			var customerId = 1;
			var orders = new List<OrderProduct>
			{
				new OrderProduct { Id = 1, ShipmentFee = 10.0m, Customer = new Customer { AccountId = customerId } }
			}.AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.AreEqual(10.0m, result.First().ShipmentFee);
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_InvalidCustomerId_ReturnsEmptyList()
		{
			// Arrange
			var customerId = 99;
			var orders = new List<OrderProduct>
			{
				new OrderProduct { Id = 1, Customer = new Customer { AccountId = 1 } }
			}.AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.IsEmpty(result);
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_NoDatabaseEntries_ReturnsEmptyList()
		{
			// Arrange
			var orders = new List<OrderProduct>().AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(1);

			// Assert
			Assert.IsEmpty(result);
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_DatabaseConnectionFails_ThrowsException()
		{
			// Arrange
			_mockContext.Setup(c => c.OrderProducts).Throws<DbUpdateException>();

			// Act & Assert
			Assert.ThrowsAsync<DbUpdateException>(async () => await _orderRepository.GetOrdersByCustomerAccountIdAsync(1));
		}

		
		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_MultipleOrdersDifferentCustomers_ReturnsOnlyMatchingOrders()
		{
			// Arrange
			var customerId = 1;
			var orders = new List<OrderProduct>
			{
				new OrderProduct { Id = 1, Customer = new Customer { AccountId = customerId } },
				new OrderProduct { Id = 2, Customer = new Customer { AccountId = 2 } }
			}.AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.AreEqual(1, result.Count());
		}

		[Test]
		public async Task GetOrdersByCustomerAccountIdAsync_ValidCustomerId_OrderWithNullFields_ReturnsOrders()
		{
			// Arrange
			var customerId = 1;
			var orders = new List<OrderProduct>
			{
				new OrderProduct { Id = 1, Customer = new Customer { AccountId = customerId }, OrderDate = null }
			}.AsQueryable();

			var mockSet = CreateMockDbSet(orders);
			_mockContext.Setup(c => c.OrderProducts).Returns(mockSet.Object);

			// Act
			var result = await _orderRepository.GetOrdersByCustomerAccountIdAsync(customerId);

			// Assert
			Assert.IsNotEmpty(result);
		}

		private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
		{
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			// Setup asynchronous query provider
			mockSet.As<IAsyncEnumerable<T>>()
				.Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
				.Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

			mockSet.As<IQueryable<T>>()
				.Setup(m => m.Provider)
				.Returns(new TestAsyncQueryProvider<T>(data.Provider));

			return mockSet;
		}
		public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
		{
			private readonly IEnumerator<T> _inner;

			public TestAsyncEnumerator(IEnumerator<T> inner)
			{
				_inner = inner;
			}

			public T Current => _inner.Current;

			public ValueTask DisposeAsync()
			{
				_inner.Dispose();
				return ValueTask.CompletedTask;
			}

			public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
		}

		public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
		{
			private readonly IQueryProvider _inner;

			public TestAsyncQueryProvider(IQueryProvider inner)
			{
				_inner = inner;
			}

			public IQueryable CreateQuery(Expression expression)
			{
				return new TestAsyncEnumerable<T>(expression);
			}

			public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
			{
				return new TestAsyncEnumerable<TElement>(expression);
			}

			public object Execute(Expression expression)
			{
				return _inner.Execute(expression);
			}

			public TResult Execute<TResult>(Expression expression)
			{
				return _inner.Execute<TResult>(expression);
			}

			public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
			{
				return Execute<TResult>(expression);
			}
		}

		public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
		{
			public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
			{
			}

			public TestAsyncEnumerable(Expression expression) : base(expression)
			{
			}

			public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			{
				return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
			}

			IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
		}

	}
}
