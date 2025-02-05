using ProjectManagementAPI.Data;

namespace ProjectManagementAPI.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);

        public TokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<TokenCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();

                    var expiredTokens = dbContext.RefreshTokens.Where(rt => rt.ExpiryDate < DateTime.UtcNow);
                    if (expiredTokens.Any())
                    {
                        _logger.LogInformation($"Delete {expiredTokens.Count()} expired Refresh Tokens...");
                        dbContext.RefreshTokens.RemoveRange(expiredTokens);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting expired Refresh Tokens.");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}
