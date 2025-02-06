using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Controllers;
using ProjectManagementAPI.Services;
using ProjectManagementAPI.Models;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers["User-Agent"] = "Test-Agent";

        _authController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "testuser", Password = "password123" };
        var authResponse = new AuthResponse { AccessToken = "accessToken", RefreshToken = "refreshToken" };

        _authServiceMock
            .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _authController.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAuthResponse = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal(authResponse.AccessToken, returnedAuthResponse.AccessToken);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "invaliduser", Password = "wrongpassword" };

        _authServiceMock
            .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((AuthResponse?)null);

        // Act
        var result = await _authController.Login(loginRequest);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registerRequest = new RegisterRequest { Username = "newuser", Password = "password123" };

        _authServiceMock
            .Setup(service => service.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authController.Register(registerRequest);

        // Assert
        Assert.IsType<ActionResult<RegisterResponse>>(result);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUsernameAlreadyExists()
    {
        // Arrange
        var registerRequest = new RegisterRequest { Username = "existinguser", Password = "password123" };

        _authServiceMock
            .Setup(service => service.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(false);

        // Act
        var result = await _authController.Register(registerRequest);

        // Assert
        Assert.IsType<ActionResult<RegisterResponse>>(result);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "validRefreshToken" };
        var authResponse = new AuthResponse { AccessToken = "newAccessToken", RefreshToken = "newRefreshToken" };

        _authServiceMock
            .Setup(service => service.RefreshTokenAsync(It.IsAny<RefreshRequest>()))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _authController.RefreshToken(refreshRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAuthResponse = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal(authResponse.AccessToken, returnedAuthResponse.AccessToken);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnUnauthorized_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "invalidRefreshToken" };

        _authServiceMock
            .Setup(service => service.RefreshTokenAsync(It.IsAny<RefreshRequest>()))
            .ReturnsAsync((AuthResponse?)null);

        // Act
        var result = await _authController.RefreshToken(refreshRequest);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenLogoutIsSuccessful()
    {
        // Arrange
        var logoutRequest = new LogoutRequest { RefreshToken = "validRefreshToken" };

        _authServiceMock
            .Setup(service => service.LogoutAsync(It.IsAny<LogoutRequest>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authController.Logout(logoutRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var logoutRequest = new LogoutRequest { RefreshToken = "invalidRefreshToken" };

        _authServiceMock
            .Setup(service => service.LogoutAsync(It.IsAny<LogoutRequest>()))
            .ReturnsAsync(false);

        // Act
        var result = await _authController.Logout(logoutRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
