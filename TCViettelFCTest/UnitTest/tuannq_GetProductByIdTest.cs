using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Controllers;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using TCViettelFC_API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Internal;
using CloudinaryDotNet.Actions;
using TCViettelFC_API.Dtos.Product;
using Newtonsoft.Json;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class tuannq_GetProductByIdTest
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;
        private Mock<ICloudinarySetting> _mockCloudinary;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockCloudinary = new Mock<ICloudinarySetting>();
        }

       

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductEqual1()
        {
            using var context = new Sep490G53Context(_dbContextOptions);

            var product = new Product
            {
                ProductId = 1 ,
                ProductName = "Product 1",
                SeasonId = 1,
                CategoryId = 1,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Material = "Cotton",
                Status = 1,
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var result = await repository.GetProductByIdAsync(1);

            Assert.IsNotNull(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.IsNotNull(data);

            Assert.AreEqual(1, (int)data.Product.ProductId);
            Assert.AreEqual("Product 1", (string)data.Product.ProductName);
        }

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            using var context = new Sep490G53Context(_dbContextOptions);


            var product2 = new Product
            {
                ProductId = 2,
                ProductName = "Product 2",
                SeasonId = 1,
                CategoryId = 1,
                Description = "Test Product Description",
                Price = 2500,
                Size = "L",
                Material = "Cotton 100%",
                Status = 1,
            };
            context.Products.Add(product2);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var result = await repository.GetProductByIdAsync(2);

            Assert.IsNotNull(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.IsNotNull(data);

            Assert.AreEqual(2, (int)data.Product.ProductId);
            Assert.AreEqual("Product 2", (string)data.Product.ProductName);
        }


        [Test]
        public async Task GetProductByIdAsync_ShouldReturnException_WhenIdEqual0()
        {
            using var context = new Sep490G53Context(_dbContextOptions);


            var product2 = new Product
            {
                ProductId = 2,
                ProductName = "Product 2",
                SeasonId = 1,
                CategoryId = 1,
                Description = "Test Product Description",
                Price = 2500,
                Size = "L",
                Material = "Cotton 100%",
                Status = 1,
            };
            context.Products.Add(product2);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<NullReferenceException>(() =>
               repository.GetProductByIdAsync(0));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("ProductId not valid", exception.Message);

        }


        [Test]
        public async Task GetProductByIdAsync_ShouldReturnException_WhenIdNegative()
        {
            using var context = new Sep490G53Context(_dbContextOptions);


            var product2 = new Product
            {
                ProductId = 2,
                ProductName = "Product 2",
                SeasonId = 1,
                CategoryId = 1,
                Description = "Test Product Description",
                Price = 2500,
                Size = "L",
                Material = "Cotton 100%",
                Status = 1,
            };
            context.Products.Add(product2);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<NullReferenceException>(() =>
               repository.GetProductByIdAsync(-1));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("ProductId not valid", exception.Message);

        }



    }
}