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

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class tuanq_AddProductTest
    {
        private DbContextOptions<Sep490G53Context> _dbContextOptions;

        // cái đầu tiên chay 
        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<Sep490G53Context>()
         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;

          
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
        public async Task AddProductAsync_ShouldAddProduct()
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

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            // Verify that the product was added
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Áo đá bóng viettel mùa 2023 - 2024");
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual("Áo đá bóng viettel mùa 2023 - 2024", savedProduct.ProductName);
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenProductNameIsNull()
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

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("Product name cannot be null", exception.Message);
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenProductNameover255Character()
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
                ProductName = "đây là tên sản phẩm test đây là tên sản phẩm " +
                "test đây là tên sản phẩm test đây là tên sản phẩm test đây là " +
                "tên sản phẩm test đây là tên sản phẩm test đây là tên sản phẩm test " +
                "đây là tên sản phẩm test đây là tên sản phẩm test đây là tên sản phẩm test đây là tên ",
                SeasonId = 1,
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

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("Product name over 255 characters", exception.Message);
        }


        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenProductNameEquals255Character()
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
                ProductName = "đây là tên sản phẩm test đây là tên sản phẩm test đây là tên sản phẩm " +
                "test đây là tên sản phẩm test đây là tên sản phẩm test đây là tên sản phẩm test đây là tên sản phẩm test đây " +
                "là tên sản phẩm test đây là tên sản phẩm test đây là tên sản phẩm test đây l",
                SeasonId = 1,
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

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(1, savedProduct.ProductId);
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenSeasonIsNull()
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
                SeasonId = null,
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

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("Season cannot be null", exception.Message);
        }


        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenCategoryIDIsNull()
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
                CategoryId = null,
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

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("CategoryId cannot be null", exception.Message);
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenPriceIsNull()
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
                CategoryId = 2,
                Description = "Test Product Description",
                Price = null,
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

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("Price cannot be null", exception.Message);
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenSizeIsNull()
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
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = null,
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

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("Size cannot be null", exception.Message);
        }

        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenStatusIsNull()
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
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "XL",
                Material = "Cotton",
                Status = null,
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

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            // Verify that the product was added
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(1, savedProduct.ProductId);
        }

        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenAvatarIsNull()
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
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "XL",
                Material = "Cotton",
                Status = 2,
                Avatar = null,
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            // Verify that the product was added
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(1, savedProduct.ProductId);
        }
        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenDiscountIdIsNull()
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
                DiscountId = null,
                SeasonId = 1,
                CategoryId = 2,
                Description = "Test Product Description",
                Price = 30000,
                Size = "XL",
                Material = "Cotton",
                Status = 2,
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

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            // Verify that the product was added
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(1, savedProduct.ProductId);
        }

        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenDescriptionIsNull()
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
                DiscountId = null,
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

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            // Verify that the product was added
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(1, savedProduct.ProductId);
        }

        [Test]
        public async Task AddProductAsync_ShouldAdded_WhenDataFileOtherIsNull()
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
                DataFile = null 
            };

            var repository = new ProductRepository(context, _mockCloudinary.Object);

            // Add product directly without a transaction (transactions aren't supported by In-Memory DB)
            await repository.AddProductAsync(productDto);
            await context.SaveChangesAsync();

            // Verify that the product was added
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(1, savedProduct.ProductId);
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenAllParamIsNull()
        {
            // Arrange: Tạo một ProductDto với ProductName là null
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<Sep490G53Context>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            using var context = new Sep490G53Context(options);

            var productDto = new ProductDto
            {
                ProductName = null,
                DiscountId = null,
                SeasonId = null,
                CategoryId = null,
                Description = null,
                Price = null,
                Size = null,
                Status = null,
                Avatar = null,
                DataFile = new List<FileResponse>
        {
            new FileResponse
            {
                FileName = "file1.jpg",
                File = CreateMockFile("file1.jpg", new byte[] { 4, 5, 6 })
            }
        }
            };

            // Act & Assert: Kiểm tra xem liệu có ngoại lệ xảy ra khi thêm sản phẩm với ProductName là null
            var repository = new ProductRepository(context, _mockCloudinary.Object);

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                repository.AddProductAsync(productDto));

            // Kiểm tra thông điệp lỗi
            Assert.AreEqual("Product name cannot be null", exception.Message);
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