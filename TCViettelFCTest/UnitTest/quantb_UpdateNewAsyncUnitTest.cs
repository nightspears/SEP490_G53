using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class quantb_UpdateNewAsyncUnitTest
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
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Bỏ qua cảnh báo transaction
                .Options;

            _context = new Sep490G53Context(_options);

            _cloudinaryMock = new Mock<ICloudinarySetting>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var configurationMock = new Mock<IConfiguration>();

            _newRepository = new NewRepository(_context, configurationMock.Object, _httpContextAccessorMock.Object, _cloudinaryMock.Object);
        }
        [Test]
        public async Task UpdateNewAsync_ShouldReturnTrue_WhenAllFieldsAreValid()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("valid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream()); 

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object  
            };

          
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") }); 

            
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);

            
            Assert.IsTrue(result);
        }


        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenIdIsNull()
        {
            
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("valid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream()); 
            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object  
            };

           
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") });

         
            var result = await _newRepository.UpdateNewsAsync(0, updateDto);  

        
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenCreatorIdIsNull()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("valid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream()); 

            var updateDto = new UpdateNewDto
            {
                CreatorId = null,  
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object  
            };

            
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") }); 

            
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);  

            
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateNewAsync_ShouldReturnTrue_WhenNewsCategoryIdIsNull()
        {
            
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("valid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream()); 

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = null,  
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object  
            };

           
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") }); 

          
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);  

           
            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateNewAsync_ShouldReturnTrue_WhenTitleIsNull()
        {
        
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("valid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream());

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = null, 
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object  
            };

          
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") }); 
 
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);  
            
            Assert.IsTrue(result);
        }
      


        [Test]
        public async Task UpdateNewAsync_ShouldReturnTrue_WhenImageIsNull()
        {
            
            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = null  
            };

           
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);

            
            Assert.IsTrue(result);
        }
        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenImageIsInvalid()
        {
            
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("invalid_image.txt"); 
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream());

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object  
            };

           
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Throws(new Exception("Invalid image format"));

            
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);

           
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenTitleExceedsMaxLength()
        {
            
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("valid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream()); 

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = new string('T', 256), 
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object 
            };

        
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") }); 
           
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);

          
            Assert.IsFalse(result);  
        }


        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenCloudinaryUploadFails()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("invalid_image.jpg");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream()); 

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = formFileMock.Object 
            };

           
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Throws(new Exception("Cloudinary upload failed"));

            var result = await _newRepository.UpdateNewsAsync(1, updateDto);


            Assert.IsFalse(result);  
        }
        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenImageIsTooLarge()
        {
            
            var formFileMock = new Mock<IFormFile>();

            formFileMock.Setup(f => f.FileName).Returns("large_image.jpg");

 
            byte[] largeFileData = new byte[20 * 1024 * 1024];  

            formFileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream(largeFileData));

            formFileMock.Setup(f => f.Length).Returns(largeFileData.Length);

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = null,
                Image = formFileMock.Object  
            };

            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Returns(new ImageUploadResult { SecureUrl = new Uri("https://validurl.com/valid_image.jpg") });

            var result = await _newRepository.UpdateNewsAsync(1, updateDto);


            Assert.IsFalse(result);
        }


        [Test]
        public async Task UpdateNewAsync_ShouldReturnFalse_WhenImageFormatIsInvalid()
        {
            // Arrange
            var invalidImageFile = new Mock<IFormFile>();
            invalidImageFile.Setup(f => f.FileName).Returns("invalid_format_image.xyz"); 

            var updateDto = new UpdateNewDto
            {
                CreatorId = 1,
                NewsCategoryId = 1,
                Title = "Thể Công-Viettel bổ sung 3 gương mặt mới cho mùa giải 24/25",
                Content = "Chiều 31/7, CLB Thể Công – Viettel chính thức...",
                Image = invalidImageFile.Object  
            };

          
            _cloudinaryMock.Setup(cloudinary => cloudinary.CloudinaryUpload(It.IsAny<IFormFile>()))
                .Throws(new Exception("Unsupported image format"));

          
            var result = await _newRepository.UpdateNewsAsync(1, updateDto);

         
            Assert.IsFalse(result);
        }




    }
}
