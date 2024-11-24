using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Controllers;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class ProductUnitTest
    {
        private Mock<IProductRepository> _mockProductRepo;
        private ProductController _controller;

        // cái đầu tiên chay 
        [SetUp]
        public void Setup()
        {
            // Dùng cái mock để nó không ảnh hưởng đến dữ liệu chạy 
            _mockProductRepo = new Mock<IProductRepository>();

            // Khởi tạo controller với mock
            _controller = new ProductController(_mockProductRepo.Object);
        }

        // Phương thức TearDown sẽ được gọi sau mỗi TC
        [TearDown]
        public void TearDown()
        {
            // Cái này cho đỡ đầy bộ nhớ 
            _mockProductRepo = null;
            _controller = null;
        }

        // đây là 1 TestCase cụ thể là TC của GetProduct trong IProductRespo
        [Test]
        public async Task GetProductAsyncUnitTest()
        {
            // Lưu data tạm trước  từ giờ sẽ được gọi là ( Arrange )

            //data tạm 
            var mockProductList = new List<ProductResponse>
            {
                new ProductResponse { ProductId  = 1, ProductName  = "Product 1" , Price = 1000000},
                new ProductResponse { ProductId  = 2, ProductName  = "Product 2" , Price = 2002200},
                new ProductResponse { ProductId  = 3, ProductName  = "Product 2" , Price = 2002200},
                new ProductResponse { ProductId  = 4, ProductName  = "Product 2" , Price = 2002200},
                new ProductResponse { ProductId  = 5, ProductName  = "Product 2" , Price = 2002200},
            };
            //lưu vào mock 
            _mockProductRepo.Setup(repo => repo.GetProductAsync()).ReturnsAsync(mockProductList);

            // Chạy Fuction  từ giờ sẽ được gọi là ( Action )
            var result = await _controller.GetProduct();

            // Check xem Fuction có hoạt động không  từ giờ sẽ được gọi là ( Assert )
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);

            Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as List<ProductResponse>;

            Assert.AreEqual(5, returnValue.Count);
        }
    }
}