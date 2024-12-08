using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using NUnit.Framework;
using Match = TCViettelFC_API.Models.Match;

namespace TCViettelFCTest.UnitTest
{
    [TestFixture]
    public class GetMatchesNotStartYetAsyncUnitTest_QuanTB
    {
        private MatchRepository _matchService;
        private DbContextOptions<Sep490G53Context> _options;
        private Sep490G53Context _context;

        [SetUp]
        public void SetUp()
        {
            // Thiết lập InMemory DbContext
            _options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Khởi tạo một phiên bản context, không dùng using để tránh bị Dispose
            _context = new Sep490G53Context(_options);

            // Khởi tạo MatchRepository với context đã tạo
            _matchService = new MatchRepository(_context, null, null);
            // Xóa hết dữ liệu trong database trước mỗi test case
            _context.Matches.RemoveRange(_context.Matches);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetMatchesNotStartYetAsync_ShouldReturnEmptyList_WhenNoMatchesMatchConditions()
        {
            // Arrange: Không có trận đấu nào trong database
            _context.Matches.RemoveRange(_context.Matches);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchesNotStartYetAsync();

            // Assert
            Assert.IsEmpty(result); // Kỳ vọng danh sách rỗng
        }

        [Test]
        public async Task GetMatchesNotStartYetAsync_ShouldReturnMatchesWithin8Days()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var matches = new List<Match>
    {
        new Match { Id = 1, MatchDate = currentDate.AddDays(2), IsHome = true, Status = 1 }  // 2 ngày tới
        // Bạn chỉ thêm một trận đấu tại đây thay vì hai trận
    };

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchesNotStartYetAsync();

            // Assert
            Assert.AreEqual(1, result.Count); // Kỳ vọng chỉ có 1 trận đấu
            Assert.AreEqual(1, result[0].Id); // Trận đầu tiên có ID là 1
        }


        [Test]
        public async Task GetMatchesNotStartYetAsync_ShouldReturnEmptyList_WhenMatchDateHasPassed()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var matches = new List<Match>
            {
                new Match { Id = 1, MatchDate = currentDate.AddDays(-1), IsHome = true, Status = 1 },
                new Match { Id = 2, MatchDate = currentDate.AddDays(-2), IsHome = true, Status = 1 }
            };

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchesNotStartYetAsync();

            // Assert
            Assert.IsEmpty(result); // Kỳ vọng danh sách rỗng
        }

        [Test]
        public async Task GetMatchesNotStartYetAsync_ShouldReturnEmptyList_WhenNoHomeMatches()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var matches = new List<Match>
            {
                new Match { Id = 1, MatchDate = currentDate.AddDays(2), IsHome = false, Status = 1 }
            };

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchesNotStartYetAsync();

            // Assert
            Assert.IsEmpty(result); // Kỳ vọng danh sách rỗng
        }

        [Test]
        public async Task GetMatchesNotStartYetAsync_ShouldReturnEmptyList_WhenMatchStatusIsNot1()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var matches = new List<Match>
            {
                new Match { Id = 1, MatchDate = currentDate.AddDays(2), IsHome = true, Status = 0 },
                new Match { Id = 2, MatchDate = currentDate.AddDays(5), IsHome = true, Status = 2 }
            };

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchesNotStartYetAsync();

            // Assert
            Assert.IsEmpty(result); // Kỳ vọng danh sách rỗng
        }

        [Test]
        public async Task GetMatchesNotStartYetAsync_ShouldReturnEmptyList_WhenMatchDateIsOutOf8Days()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var matches = new List<Match>
            {
                new Match { Id = 1, MatchDate = currentDate.AddDays(9), IsHome = true, Status = 1 }
            };

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchesNotStartYetAsync();

            // Assert
            Assert.IsEmpty(result); // Kỳ vọng danh sách rỗng
        }

        // Dọn dẹp sau mỗi lần kiểm thử (Nếu cần thiết)
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
