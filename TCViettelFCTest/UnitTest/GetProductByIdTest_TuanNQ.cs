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
    public class GetProductByIdTest_TuanNQ
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
        }

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            using var context = new Sep490G53Context(_dbContextOptions);

            // Add product to the in-memory database
            var product = new Product
            {
                ProductName = "Product 2",
                SeasonId = 1,
                CategoryId = 1,
                Description = "Test Product Description",
                Price = 2500,
                Size = "L",
                Material = "Cotton 100%",
                Status = 1, // Active status
            };
            context.Products.Add(product);

            // Add associated ProductFile to the in-memory database
            var productFile = new ProductFile
            {
                ProductId = 2,
                FileName = "TestFile.jpg",
                Status = 1, // Active status
            };
            context.ProductFiles.Add(productFile);

            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Call the method
            var result = await repository.GetProductByIdAsync(2);

            // Assert results
            Assert.IsNotNull(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.IsNotNull(data);

            // Validate the product fields
            Assert.AreEqual(2, (int)data.Product.ProductId);
            Assert.AreEqual("Product 2", (string)data.Product.ProductName);
            Assert.AreEqual("L", (string)data.Product.Size);

            // Validate the associated files
            Assert.IsNotNull(data.PFile);
            Assert.IsTrue(((IEnumerable<dynamic>)data.PFile).Any(x => x.FileName == "TestFile.jpg"));
        }

    }
}