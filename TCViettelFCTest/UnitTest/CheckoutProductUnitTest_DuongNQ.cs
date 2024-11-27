using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCViettelFC_API.Controllers;
using TCViettelFC_API.Models;
using TCViettelFC_API.Dtos.CheckOut;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TCViettelFCTest.UnitTest
{
	internal class CheckoutProductUnitTest_DuongNQ
	{
		private Mock<ITicketUtilRepository> _mockTicketUtilRepository;
		private CheckoutController _checkoutController;
		private Sep490G53Context _context;

		[SetUp]
		public void SetUp()
		{
			var options = new DbContextOptionsBuilder<Sep490G53Context>()
				.UseInMemoryDatabase("TestDatabase")
				.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore the transaction warning
				.Options;
			_context = new Sep490G53Context(options);

			// Mock ITicketUtilRepository (if needed for testing)
			_mockTicketUtilRepository = new Mock<ITicketUtilRepository>();

			// Initialize the CheckoutController with the actual DbContext
			_checkoutController = new CheckoutController(_context, _mockTicketUtilRepository.Object);
		}
		[Test]
		public async Task CreateOrder_ShouldReturnOk_WhenOrderIsCreatedSuccessfully()
		{
			// Arrange: Prepare test data
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 100.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
				{
					new OrderProductDetailDTO
					{
						ProductId = 1,
						ProductName = "Shirt",
						Avatar = "avatar.jpg",
						PlayerId = 2,
						CustomShirtNumber = "10",
						CustomShirtName = "Custom Name",
						Size = "L",
						Quantity = 2,
						Price = 50.0m
					}
				},
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify the result is Ok
			Assert.IsInstanceOf<OkObjectResult>(result);
			var okResult = result as OkObjectResult;
			Assert.AreEqual("Order created successfully", okResult.Value?.GetType().GetProperty("Message")?.GetValue(okResult.Value));
		}


		[Test]
		public async Task CreateOrder_ShouldReturnBadRequest_WhenCustomerIsMissing()
		{
			// Arrange: Prepare test data with missing customer data
			var request = new CreateOrderRequest
			{
				Customer = null,  // Customer is missing
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 100.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO
			{
				ProductId = 1,
				ProductName = "Shirt",
				Avatar = "avatar.jpg",
				PlayerId = 2,
				CustomShirtNumber = "10",
				CustomShirtName = "Custom Name",
				Size = "L",
				Quantity = 2,
				Price = 50.0m
			}
		},
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is a BadRequest
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Customer data is required.", badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
		}

		[Test]
		public async Task CreateOrder_ShouldReturnBadRequest_WhenAddressIsMissing()
		{
			// Arrange: Prepare test data with missing address data
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					FullName = "John Doe",
					Email = "johndoe@example.com",
					Phone = "1234567890"
				},
				Address = null,  // Address is missing
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD002",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 100.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO
			{
				ProductId = 1,
				ProductName = "Shirt",
				Avatar = "avatar.jpg",
				PlayerId = 2,
				CustomShirtNumber = "10",
				CustomShirtName = "Custom Name",
				Size = "L",
				Quantity = 2,
				Price = 50.0m
			}
		},
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is a BadRequest
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Address data is required.", badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
		}

		[Test]
		public async Task CreateOrder_ShouldCallSendOrderConfirmationEmail_WhenOrderIsCreatedSuccessfully()
		{
			// Arrange: Prepare test data
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 100.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO
			{
				ProductId = 1,
				ProductName = "Shirt",
				Avatar = "avatar.jpg",
				PlayerId = 2,
				CustomShirtNumber = "10",
				CustomShirtName = "Custom Name",
				Size = "L",
				Quantity = 2,
				Price = 50.0m
			}
		},
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that SendOrderConfirmationEmailAsync was called
			_mockTicketUtilRepository.Verify(m => m.SendOrderConfirmationEmailAsync(request), Times.Once);
		}
		[Test]
		public async Task CreateOrder_ShouldReturnBadRequest_WhenOrderProductDetailsIsMissing()
		{
			// Arrange: Prepare test data with missing OrderProductDetails
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 100.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>(), // Empty OrderProductDetails list
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is a BadRequest
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Order product details are required.", badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
		}
		[Test]
		public async Task CreateOrder_ShouldReturnBadRequest_WhenPaymentGatewayIsMissing()
		{
			// Arrange: Prepare test data with missing PaymentGateway
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 100.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO { ProductId = 1, Quantity = 2, Price = 50.0m }
		},
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = null // Missing PaymentGateway
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is a BadRequest
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Payment gateway is required.", badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
		}

		[Test]
		public async Task CreateOrder_ShouldCreateMultipleOrders_WhenValidDataIsProvided()
		{
			// Arrange: Prepare multiple valid requests
			var requests = new List<CreateOrderRequest>
	{
		new CreateOrderRequest
		{
			Customer = new CustomerDTO
			{
				AccountId = 1,
				Email = "customer1@example.com",
				Phone = "123456789",
				FullName = "John Doe"
			},
			Address = new AddressDTO
			{
				City = "City1",
				CityName = "CityName1",
				District = "District1",
				DistrictName = "DistrictName1",
				Ward = "Ward1",
				WardName = "WardName1",
				DetailedAddress = "123 Street"
			},
			OrderProduct = new OrderProductDTO
			{
				OrderCode = "ORD001",
				OrderDate = DateTime.Now,
				ShipmentFee = 10.0m,
				TotalPrice = 100.0m
			},
			OrderProductDetails = new List<OrderProductDetailDTO>
			{
				new OrderProductDetailDTO
				{
					ProductId = 1,
					ProductName = "Shirt",
					Avatar = "avatar.jpg",
					PlayerId = 2,
					CustomShirtNumber = "10",
					CustomShirtName = "Custom Name",
					Size = "L",
					Quantity = 2,
					Price = 50.0m
				}
			},
			Payment = new PaymentDTO
			{
				TotalAmount = 120.0m,
				PaymentGateway = "VnPay"
			}
		},
		new CreateOrderRequest
		{
			Customer = new CustomerDTO
			{
				AccountId = 2,
				Email = "customer2@example.com",
				Phone = "987654321",
				FullName = "Jane Smith"
			},
			Address = new AddressDTO
			{
				City = "City2",
				CityName = "CityName2",
				District = "District2",
				DistrictName = "DistrictName2",
				Ward = "Ward2",
				WardName = "WardName2",
				DetailedAddress = "456 Street"
			},
			OrderProduct = new OrderProductDTO
			{
				OrderCode = "ORD002",
				OrderDate = DateTime.Now,
				ShipmentFee = 15.0m,
				TotalPrice = 150.0m
			},
			OrderProductDetails = new List<OrderProductDetailDTO>
			{
				new OrderProductDetailDTO
				{
					ProductId = 2,
					ProductName = "Jacket",
					Avatar = "avatar2.jpg",
					PlayerId = 3,
					CustomShirtNumber = "20",
					CustomShirtName = "Custom Jacket",
					Size = "M",
					Quantity = 1,
					Price = 150.0m
				}
			},
			Payment = new PaymentDTO
			{
				TotalAmount = 165.0m,
				PaymentGateway = "VnPay"
			}
		}
	};

			foreach (var request in requests)
			{
				// Act: Call the method under test for each order
				var result = await _checkoutController.CreateOrder(request);

				// Assert: Verify the result is Ok for each order
				Assert.IsInstanceOf<OkObjectResult>(result);
				var okResult = result as OkObjectResult;
				Assert.AreEqual("Order created successfully", okResult.Value?.GetType().GetProperty("Message")?.GetValue(okResult.Value));
			}
		}


		[Test]
		public async Task CreateOrder_ShouldReturnBadRequest_WhenOrderProductIsMissing()
		{
			// Arrange: Prepare test data with missing OrderProduct
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = null,  // Missing OrderProduct
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO { ProductId = 1, Quantity = 2, Price = 50.0m }
		},
				Payment = new PaymentDTO
				{
					TotalAmount = 120.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is a BadRequest
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Order product data is required.", badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
		}
		[Test]
		public async Task CreateOrder_ShouldHandleLargeOrderSuccessfully()
		{
			// Arrange: Prepare test data with a large number of products
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = 1000.0m
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO { ProductId = 1, Quantity = 10, Price = 50.0m },
			new OrderProductDetailDTO { ProductId = 2, Quantity = 20, Price = 25.0m },
			new OrderProductDetailDTO { ProductId = 3, Quantity = 30, Price = 15.0m }
		},
				Payment = new PaymentDTO
				{
					TotalAmount = 1500.0m,
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is Ok
			Assert.IsInstanceOf<OkObjectResult>(result);
			var okResult = result as OkObjectResult;
			Assert.AreEqual("Order created successfully", okResult.Value?.GetType().GetProperty("Message")?.GetValue(okResult.Value));
		}

		[Test]
		public async Task CreateOrder_ShouldReturnBadRequest_WhenTotalPriceIsNegative()
		{
			// Arrange: Prepare test data with negative TotalPrice
			var request = new CreateOrderRequest
			{
				Customer = new CustomerDTO
				{
					AccountId = 1,
					Email = "customer@example.com",
					Phone = "123456789",
					FullName = "John Doe"
				},
				Address = new AddressDTO
				{
					City = "City",
					CityName = "CityName",
					District = "District",
					DistrictName = "DistrictName",
					Ward = "Ward",
					WardName = "WardName",
					DetailedAddress = "123 Street"
				},
				OrderProduct = new OrderProductDTO
				{
					OrderCode = "ORD001",
					OrderDate = DateTime.Now,
					ShipmentFee = 10.0m,
					TotalPrice = -100.0m // Invalid negative TotalPrice
				},
				OrderProductDetails = new List<OrderProductDetailDTO>
		{
			new OrderProductDetailDTO { ProductId = 1, Quantity = 2, Price = 50.0m }
		},
				Payment = new PaymentDTO
				{
					TotalAmount = -120.0m, // Invalid negative Payment amount
					PaymentGateway = "VnPay"
				}
			};

			// Act: Call the method under test
			var result = await _checkoutController.CreateOrder(request);

			// Assert: Verify that the result is a BadRequest
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Total price and payment amount cannot be negative.", badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value));
		}

	}
}
