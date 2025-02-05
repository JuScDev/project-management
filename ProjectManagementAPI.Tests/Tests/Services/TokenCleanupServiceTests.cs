using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;

public class TokenCleanupServiceTests
{
    private readonly Mock<ILogger<TokenCleanupService>> _loggerMock;
    private readonly IServiceScopeFactory _scopeFactory;

    public TokenCleanupServiceTests()
    {
        _loggerMock = new Mock<ILogger<TokenCleanupService>>();

        var services = new ServiceCollection();
        services.AddDbContext<ProjectDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task TokenCleanupService_ShouldDeleteExpiredTokens()
    {
        // Arrange
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();

        dbContext.RefreshTokens.AddRange(new[]
        {
            new RefreshToken { Id = 1, ExpiryDate = DateTime.UtcNow.AddHours(-2) }, // Expired
            new RefreshToken { Id = 2, ExpiryDate = DateTime.UtcNow.AddHours(2) }  // Valid
        });

        await dbContext.SaveChangesAsync();

        var cleanupService = new TokenCleanupService(_scopeFactory, _loggerMock.Object);

        // Act
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(1)); // End after 1 second
        await cleanupService.StartAsync(cts.Token);

        // Assert
        var remainingTokens = dbContext.RefreshTokens.ToList();
        Assert.Single(remainingTokens); // Only one token should be left
        Assert.Equal(2, remainingTokens.First().Id); // The valid token should be left
    }
}
