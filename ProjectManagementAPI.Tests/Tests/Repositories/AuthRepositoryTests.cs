using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Repositories;
using Xunit;

public class AuthRepositoryTests : IDisposable
{
    private readonly ProjectDbContext _context;
    private readonly AuthRepository _authRepository;

    public AuthRepositoryTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique DB for each test
            .Options;

        _context = new ProjectDbContext(options);
        _authRepository = new AuthRepository(_context);
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "hashedpassword" };

        // Act
        await _authRepository.AddUserAsync(user);
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");

        // Assert
        Assert.NotNull(dbUser);
        Assert.Equal("testuser", dbUser.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "existinguser", PasswordHash = "hashedpassword" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authRepository.GetUserByUsernameAsync("existinguser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("existinguser", result.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _authRepository.GetUserByUsernameAsync("nonexistentuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddRefreshTokenAsync_ShouldStoreRefreshToken()
    {
        // Arrange
        var refreshToken = new RefreshToken { Token = "test-token", Username = "testuser", ExpiryDate = DateTime.UtcNow.AddDays(7) };

        // Act
        await _authRepository.AddRefreshTokenAsync(refreshToken);
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "test-token");

        // Assert
        Assert.NotNull(storedToken);
        Assert.Equal("test-token", storedToken.Token);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_ShouldReturnToken_WhenTokenExists()
    {
        // Arrange
        var refreshToken = new RefreshToken { Token = "valid-token", Username = "testuser", ExpiryDate = DateTime.UtcNow.AddDays(7) };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authRepository.GetRefreshTokenAsync("valid-token");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("valid-token", result.Token);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Act
        var result = await _authRepository.GetRefreshTokenAsync("nonexistent-token");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveRefreshTokenAsync_ShouldDeleteToken()
    {
        // Arrange
        var refreshToken = new RefreshToken { Token = "removable-token", Username = "testuser", ExpiryDate = DateTime.UtcNow.AddDays(7) };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        await _authRepository.RemoveRefreshTokenAsync(refreshToken);
        var result = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "removable-token");

        // Assert
        Assert.Null(result);
    }

    // Dispose database after tests
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
