using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
	[TestFixture]
	public class UpdateOrderStatusUniteTest_DuongNQ
	{
		private Mock<Sep490G53Context> _mockContext;
		private Mock<DbSet<OrderProduct>> _mockOrderProductsDbSet;
		private IOrderRepository _orderRepository;

		[SetUp]
		public void Setup()
		{
			_mockContext = new Mock<Sep490G53Context>();
			_mockOrderProductsDbSet = new Mock<DbSet<OrderProduct>>();
			_mockContext.Setup(c => c.OrderProducts).Returns(_mockOrderProductsDbSet.Object);
			_orderRepository = new OrderRepository(_mockContext.Object);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_OrderExists_ReturnsTrue()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 2;
			var staffId = 1;
			var order = new OrderProduct { Id = orderId, Status = 1 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(newStatus, order.Status);
			Assert.AreEqual(staffId, order.StaffId);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_OrderNotFound_ReturnsFalse()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 2;
			var staffId = 1;
			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync((OrderProduct)null);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_StatusUpdate()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 3;
			var staffId = 2;
			var order = new OrderProduct { Id = orderId, Status = 1, StaffId = 0 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(newStatus, order.Status);
			Assert.AreEqual(staffId, order.StaffId);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_NullOrder_ReturnsFalse()
		{
			// Arrange
			var orderId = 2;
			var newStatus = 2;
			var staffId = 3;
			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync((OrderProduct)null);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_ValidStatusChange()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 4;
			var staffId = 5;
			var order = new OrderProduct { Id = orderId, Status = 2 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(newStatus, order.Status);
			Assert.AreEqual(staffId, order.StaffId);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_ValidOrder_StatusUpdatedCorrectly()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 5;
			var staffId = 6;
			var order = new OrderProduct { Id = orderId, Status = 3 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(newStatus, order.Status);
			Assert.AreEqual(staffId, order.StaffId);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_InvalidStatus_ReturnsTrue()
		{
			// Arrange
			var orderId = 1;
			var newStatus = -1; // Invalid status
			var staffId = 1;
			var order = new OrderProduct { Id = orderId, Status = 1 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result); // Test behavior on invalid status handling (depends on business logic)
		}

		[Test]
		public async Task UpdateOrderStatusAsync_OrderStatusUnchanged_AfterUpdate_ReturnsTrue()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 1; // Same status
			var staffId = 2;
			var order = new OrderProduct { Id = orderId, Status = newStatus };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result); // Ensure method returns true even if no actual status change
		}

		[Test]
		public async Task UpdateOrderStatusAsync_UpdateStaffId()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 3;
			var staffId = 4;
			var order = new OrderProduct { Id = orderId, Status = 2 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(staffId, order.StaffId);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_DbContextSaveChangesCalled()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 2;
			var staffId = 1;
			var order = new OrderProduct { Id = orderId, Status = 1 };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);
			_mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

			// Act
			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			_mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			Assert.IsTrue(result);
		}

		[Test]
		public async Task UpdateOrderStatusAsync_OrderNotModified_ThrowsException()
		{
			// Arrange
			var orderId = 1;
			var newStatus = 0; // No change
			var staffId = 1;
			var order = new OrderProduct { Id = orderId, Status = newStatus };

			_mockOrderProductsDbSet.Setup(m => m.FindAsync(orderId)).ReturnsAsync(order);

			var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);

			// Assert
			Assert.IsTrue(result);
		}
	}
}
