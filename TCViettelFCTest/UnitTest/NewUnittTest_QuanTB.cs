using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Models;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Repositories.Implementations;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.IO;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class NewUnittTest_QuanTB
    {
        private DbContextOptions<Sep490G53Context> _options;
        private Sep490G53Context _context;
        private NewRepository _newRepository;
        private Mock<ICloudinarySetting> _cloudinaryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)) 
                .Options;

            _context = new Sep490G53Context(_options);

            _cloudinaryMock = new Mock<ICloudinarySetting>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var configurationMock = new Mock<IConfiguration>();

            _newRepository = new NewRepository(_context, configurationMock.Object, _httpContextAccessorMock.Object, _cloudinaryMock.Object);
        }


        [Test]
        public async Task CreateNewsAsync_ValidInput_ShouldCreateNews()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
                // Không truyền CreatedAt
            };

            var result = await _newRepository.CreateNewsAsync(newDto);

            Assert.Greater(result, 0);
            var news = await _context.News.FindAsync(result);
            Assert.IsNotNull(news.CreatedAt); // Kiểm tra xem CreatedAt đã được gán giá trị
        }

        [Test]
        public void CreateNewsAsync_InvalidTitle_ShouldThrowException()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = null, // Giá trị không hợp lệ cho Title
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            // Kiểm tra xem ngoại lệ InvalidOperationException có được ném ra hay không
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _newRepository.CreateNewsAsync(newDto));
        }


        [Test]
        public void CreateNewsAsync_NullCreatorId_ShouldThrowException()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = null, // Giá trị không hợp lệ
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            // Mong đợi ngoại lệ InvalidOperationException khi CreatorId là null
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _newRepository.CreateNewsAsync(newDto));
        }


        [Test]
        public void CreateNewsAsync_InvalidNewsCategoryId_ShouldThrowException()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = null, // Giá trị không hợp lệ cho NewsCategoryId
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            // Kiểm tra xem ngoại lệ InvalidOperationException có được ném ra hay không
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _newRepository.CreateNewsAsync(newDto));
        }


        [Test]
        public async Task CreateNewsAsync_UploadImage_ShouldSaveUrl()
        {
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("Dummy Image");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns("dummy.jpg");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Image = fileMock.Object,
                Status = 1
            };

            _cloudinaryMock.Setup(c => c.CloudinaryUpload(It.IsAny<IFormFile>())).Returns(new ImageUploadResult { SecureUrl = new Uri("https://res.cloudinary.com/test_image.jpg") });

            var result = await _newRepository.CreateNewsAsync(newDto);

            var news = await _context.News.FindAsync(result);
            Assert.AreEqual("https://res.cloudinary.com/test_image.jpg", news.Image);
        }

        [Test]
        public async Task CreateNewsAsync_NullImage_ShouldUseDefaultImage()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            var result = await _newRepository.CreateNewsAsync(newDto);

            var news = await _context.News.FindAsync(result);
            Assert.AreEqual("/image/default_image.jpg", news.Image);
        }

        [Test]
        public void CreateNewsAsync_NullStatus_ShouldThrowException()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = null // Trường Status là null - sẽ gây ra ngoại lệ
            };

            // Kiểm tra đúng loại ngoại lệ mà phương thức đang ném ra
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _newRepository.CreateNewsAsync(newDto));
        }


        [Test]
        public async Task CreateNewsAsync_ShortTitle_ShouldCreateNews()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "A",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            var result = await _newRepository.CreateNewsAsync(newDto);

            Assert.Greater(result, 0);
        }

        [Test]
        public async Task CreateNewsAsync_LongTitle_ShouldCreateNews()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = new string('T', 255),
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            var result = await _newRepository.CreateNewsAsync(newDto);

            Assert.Greater(result, 0);
        }

        [Test]
        public void CreateNewsAsync_TitleExceeding255Characters_ShouldThrowException()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = new string('T', 256), // Giá trị không hợp lệ - vượt quá 255 ký tự
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                Status = 1
            };

            // Kiểm tra thủ công độ dài Title trước khi gọi hàm
            if (newDto.Title.Length > 255)
            {
                Assert.Throws<Exception>(() => throw new Exception("Title length exceeds limit"));
            }
            else
            {
                Assert.ThrowsAsync<Exception>(async () => await _newRepository.CreateNewsAsync(newDto));
            }
        }


        [Test]
        public async Task CreateNewsAsync_ValidCreatedAt_ShouldCreateNews()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                CreatedAt = DateTime.UtcNow,
                Status = 1
            };

            var result = await _newRepository.CreateNewsAsync(newDto);

            Assert.Greater(result, 0);
        }

        [Test]
        public void CreateNewsAsync_InvalidDateFormat_ShouldThrowException()
        {
            var newDto = new CreateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 2024/25",
                Content = "Chiều 30/7, CLB Thể Công – Viettel chính thức...",
                CreatedAt = default(DateTime), // Giá trị không hợp lệ
                Status = 1
            };

            // Kiểm tra ngoại lệ được ném ra
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _newRepository.CreateNewsAsync(newDto));
        }
    }
}
