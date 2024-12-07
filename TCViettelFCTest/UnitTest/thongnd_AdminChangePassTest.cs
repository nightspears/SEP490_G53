using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;

[TestFixture]
public class UserRepositoryTests
{
    private Mock<Sep490G53Context> _mockContext;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IHttpContextAccessor> _mockContextAccessor;
    private UserRepository _userRepository;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<Sep490G53Context>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();
        _userRepository = new UserRepository(null, _mockContext.Object, _mockConfiguration.Object, _mockContextAccessor.Object);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldChangePassword_WhenValidRequest()
    {
        // Arrange
        var user = new User { UserId = 1, Password = "OldPassword123" };
        var ch = new ChangePassRequest { OldPass = "OldPassword123", NewPass = "NewPassword456" };

        _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
        SetUserClaims("1");

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(1, result);
        Assert.AreEqual("NewPassword456", user.Password);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenOldPasswordDoesNotMatch()
    {
        // Arrange
        var user = new User { UserId = 1, Password = "OldPassword123" };
        var ch = new ChangePassRequest { OldPass = "WrongPassword", NewPass = "NewPassword456" };

        _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
        SetUserClaims("1");

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenNewPasswordMatchesOldPassword()
    {
        // Arrange
        var user = new User { UserId = 1, Password = "OldPassword123" };
        var ch = new ChangePassRequest { OldPass = "OldPassword123", NewPass = "OldPassword123" };

        _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
        SetUserClaims("1");

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenUserNotFound()
    {
        // Arrange
        var ch = new ChangePassRequest { OldPass = "OldPassword123", NewPass = "NewPassword456" };

        _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null);
        SetUserClaims("1");

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(0, result);
    }

    //[Test]
    //public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenSaveChangesFails()
    //{
    //    // Arrange
    //    var user = new User { UserId = 1, Password = "OldPassword123" };
    //    var ch = new ChangePassRequest { OldPass = "OldPassword123", NewPass = "NewPassword456" };

    //    _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
    //    _mockContext.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new Exception());
    //    SetUserClaims("1");

    //    // Act
    //    var result = await _userRepository.AdminChangePasswordAsync(ch);

    //    // Assert
    //    Assert.AreEqual(0, result);
    //}

    [Test]
    public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenHttpContextIsNull()
    {
        // Arrange
        _mockContextAccessor.Setup(ca => ca.HttpContext).Returns((HttpContext)null);
        var ch = new ChangePassRequest { OldPass = "OldPassword123", NewPass = "NewPassword456" };

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenUserIdClaimMissing()
    {
        // Arrange
        _mockContextAccessor.Setup(ca => ca.HttpContext).Returns(new DefaultHttpContext());
        var ch = new ChangePassRequest { OldPass = "OldPassword123", NewPass = "NewPassword456" };

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldHandleNullChangePassRequest()
    {
        // Arrange
        SetUserClaims("1");

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(null);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task AdminChangePasswordAsync_ShouldReturnZero_WhenOldPassAndNewPassAreEmpty()
    {
        // Arrange
        var user = new User { UserId = 1, Password = "OldPassword123" };
        var ch = new ChangePassRequest { OldPass = "", NewPass = "" };

        _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
        SetUserClaims("1");

        // Act
        var result = await _userRepository.AdminChangePasswordAsync(ch);

        // Assert
        Assert.AreEqual(0, result);
    }

    private void SetUserClaims(string userId)
    {
        var claims = new[] { new Claim("UserId", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockContextAccessor.Setup(ca => ca.HttpContext).Returns(httpContext);
    }
}
