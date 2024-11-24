using Microsoft.EntityFrameworkCore;
using Moq;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class thongnd_GetAreadByIdTest
    {
        private AreaRepository _repository;
        private Mock<DbSet<Area>> _mockSet;
        private Mock<Sep490G53Context> _mockContext;

        [SetUp]
        public void Setup()
        {
            _mockSet = new Mock<DbSet<Area>>();
            _mockContext = new Mock<Sep490G53Context>();
            _mockContext.Setup(c => c.Areas).Returns(_mockSet.Object);
            _repository = new AreaRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetAreaById_ReturnsNull_WhenAreaNotFound()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Area)null);

            // Act
            var result = await _repository.GetAreaById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAreaById_ReturnsCorrectData_WhenAreaExists()
        {
            // Arrange
            var area = new Area { Id = 1, Floor = "2", Price = 300, Section = "A", Stands = "100", Status = 1 };
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(area);

            // Act
            var result = await _repository.GetAreaById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(area.Id, result.Id);
            Assert.AreEqual(area.Floor, result.Floor);
            Assert.AreEqual(area.Price, result.Price);
            Assert.AreEqual(area.Section, result.Section);
            Assert.AreEqual(area.Stands, result.Stands);
            Assert.AreEqual(area.Status, result.Status);
        }

        [Test]
        public async Task GetAreaById_HandlesZeroId_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(0)).ReturnsAsync((Area)null);

            // Act
            var result = await _repository.GetAreaById(0);

            // Assert
            Assert.IsNull(result);
        }


    }
}
