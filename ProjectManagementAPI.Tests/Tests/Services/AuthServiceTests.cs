using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Repositories;
using ProjectManagementAPI.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();

        // Mock JWT-Einstellungen für Token-Generierung
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(config => config.GetSection("JwtSettings")["SecretKey"]).Returns("SuperLongSecretKeyForUnitTesting!!");
        _configurationMock.Setup(config => config.GetSection("JwtSettings")["Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(config => config.GetSection("JwtSettings")["Audience"]).Returns("TestAudience");

        _authService = new AuthService(_configurationMock.Object, _authRepositoryMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var methodInfo = _authService.GetType().GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (methodInfo == null)
        {
            throw new InvalidOperationException("Method 'HashPassword' not found in AuthService.");
        }

        var hashedPassword = methodInfo.Invoke(_authService, new object[] { "password123" }) as string
            ?? throw new InvalidOperationException("HashPassword returned null.");

        var user = new User
        {
            Username = "testuser",
            PasswordHash = hashedPassword
        };

        var request = new LoginRequest { Username = "testuser", Password = "password123" };

        _authRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        _authRepositoryMock.Setup(repo => repo.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(request, "127.0.0.1", "TestDevice");

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new LoginRequest { Username = "wronguser", Password = "wrongpassword" };

        _authRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request, "127.0.0.1", "TestDevice");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnTrue_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new RegisterRequest { Username = "newuser", Password = "password123" };

        _authRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        _authRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFalse_WhenUsernameAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest { Username = "existinguser", Password = "password123" };
        var existingUser = new User { Username = "existinguser", PasswordHash = "hash123" };

        _authRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(request.Username))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // Arrange
        var request = new RefreshRequest { RefreshToken = "validRefreshToken" };
        var storedToken = new RefreshToken { Token = "validRefreshToken", Username = "testuser", ExpiryDate = DateTime.UtcNow.AddDays(1) };

        _authRepositoryMock.Setup(repo => repo.GetRefreshTokenAsync(request.RefreshToken))
            .ReturnsAsync(storedToken);

        _authRepositoryMock.Setup(repo => repo.RemoveRefreshTokenAsync(storedToken))
            .Returns(Task.CompletedTask);

        _authRepositoryMock.Setup(repo => repo.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RefreshTokenAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var request = new RefreshRequest { RefreshToken = "expiredRefreshToken" };
        var storedToken = new RefreshToken { Token = "expiredRefreshToken", Username = "testuser", ExpiryDate = DateTime.UtcNow.AddDays(-1) };

        _authRepositoryMock.Setup(repo => repo.GetRefreshTokenAsync(request.RefreshToken))
            .ReturnsAsync(storedToken);

        // Act
        var result = await _authService.RefreshTokenAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnTrue_WhenRefreshTokenIsValid()
    {
        // Arrange
        var request = new LogoutRequest { RefreshToken = "validToken" };
        var storedToken = new RefreshToken { Token = "validToken", Username = "testuser" };

        _authRepositoryMock.Setup(repo => repo.GetRefreshTokenAsync(request.RefreshToken))
            .ReturnsAsync(storedToken);

        _authRepositoryMock.Setup(repo => repo.RemoveRefreshTokenAsync(storedToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LogoutAsync(request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnFalse_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var request = new LogoutRequest { RefreshToken = "invalidToken" };

        _authRepositoryMock.Setup(repo => repo.GetRefreshTokenAsync(request.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var result = await _authService.LogoutAsync(request);

        // Assert
        Assert.False(result);
    }
}
