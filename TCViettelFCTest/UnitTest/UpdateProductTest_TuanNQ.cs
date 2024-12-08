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
using TCViettelFC_API.Repositories.Implementations;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using CloudinaryDotNet;
using static QRCoder.PayloadGenerator.SwissQrCode;
using Microsoft.Extensions.DependencyInjection;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class UpdateProductTest_TuanNQ
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;

        // cái đầu tiên chay 
        [SetUp]
        public void Setup()
        {
         //   _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
         //.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         //.Options;

        
        }
        [TearDown]
        public void TearDown()
        {
            _dbContextOptions = null; // Giải phóng tùy chọn
        }

        private Mock<ICloudinarySetting> _mockCloudinary;

        [SetUp]
        public void SetupMocks()
        {
            _mockCloudinary = new Mock<ICloudinarySetting>();

            // Mock upload avatar
            _mockCloudinary.Setup(c => c.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("http://example.com/avatar.jpg") });

            // Mock upload file in DataFile
            _mockCloudinary.Setup(c => c.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("http://example.com/file.jpg") });
        }


        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            using var context = new Sep490G53Context(options);

            // Create the product DTO to add to the database
            var productDto = new ProductDto
            {
              
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();

            // Verify that the product was added
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual("Áo đá bóng viettel mùa 2024 - 2025", savedProduct.ProductName);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNameIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };


            var productUpdate = new ProductDto
            {
                ProductName = null,
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(savedProduct.ProductId , productUpdate));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no Product are updated.", exception.Message);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNameover255Character()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                // ProductName quá 255 ký tự 
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                // 256 ký tự d
                ProductName = "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "dddddddddddddddddddddddddddddddddd",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");


            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(savedProduct.ProductId , productUpdate));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no Product are updated.", exception.Message);
        }


        [Test]
        public async Task UpdateProductAsync_ShouldUpdate_WhenProductNameEquals255Character()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                // ProductName bằng 255 ký tự 
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                //255 ký tự d
                ProductName = "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddd",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);

            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");
            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(255, savedProduct.ProductName.Length);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenSeasonIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Color = "Red",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };


            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = null,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(savedProduct.ProductId, productUpdate));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no Product are updated.", exception.Message);
        }


        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenCategoryIDIsNull()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 1000,
                Size = "M",
                Color = "Red",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = null,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(savedProduct.ProductId , productUpdate));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no Product are updated.", exception.Message);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenPriceIsNull()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 250000,
                Size = "M",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = null,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };


            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(savedProduct.ProductId, productUpdate));
            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no Product are updated.", exception.Message);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenSizeIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "41",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = null,
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] {6}),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };


         
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(savedProduct.ProductId, productUpdate));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("The system returns an error, no Product are updated.", exception.Message);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdate_WhenStatusIsNull()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "XL",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = null,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();
            Assert.AreEqual("Áo đá bóng viettel mùa 2024 - 2025", savedProduct.ProductName);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdate_WhenAvatarIsNull()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "XL",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = null,
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();
      
            Assert.AreEqual("Áo đá bóng viettel mùa 2024 - 2025", savedProduct.ProductName);

        }
        [Test]
        public async Task UpdateProductAsync_ShouldUpdate_WhenDiscountIdIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                DiscountId = 1,
                SeasonId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "XL",
                Material = "Cotton",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = null,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(null, savedProduct.DiscountId);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdate_WhenDescriptionIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                DiscountId = 1,
                SeasonId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 200000,
                Size = "XL",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description =null,
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(null, savedProduct.Description);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdate_WhenDataFileOtherIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                DiscountId = 1,
                SeasonId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 200000,
                Size = "XL",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1}),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };
            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1}),
                DataFile = null
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            await repository.UpdateProductAsync(savedProduct.ProductId, productUpdate);
            await context.SaveChangesAsync();
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual("Áo đá bóng viettel mùa 2024 - 2025", savedProduct.ProductName);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductIdIsZezo()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                DiscountId = 1,
                SeasonId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 200000,
                Size = "XL",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };
            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(0, productUpdate));
            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("ProductId not valid", exception.Message);
        }


        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductIdNegative()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2023 - 2024",
                DiscountId = 1,
                SeasonId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 200000,
                Size = "XL",
                Status = 1,
                Avatar = CreateMockFile("avatar.jpg", new byte[] { 1, 2, 3 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };
            var productUpdate = new ProductDto
            {
                ProductName = "Áo đá bóng viettel mùa 2024 - 2025",
                SeasonId = 1,
                DiscountId = 1,
                CategoryId = 2,
                Description = "Test Product Description 1",
                Price = 100000,
                Size = "XL",
                Material = "Cotton 100%",
                Status = 1,
                Avatar = CreateMockFile("avatar1.jpg", new byte[] { 4, 5, 6 }),
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "fileUpdate.jpg",
                File = CreateMockFile("fileUpdate.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.UpdateProductAsync(-1, productUpdate));
            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("ProductId not valid", exception.Message);
        }



        private IFormFile CreateMockFile(string fileName, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }



    }


}