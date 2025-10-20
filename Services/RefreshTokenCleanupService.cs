using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> logger;

    public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<RefreshTokenCleanupService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookStoreContext>();

            var expiredTokens = await context.RefreshTokens
                .Where(t => t.ExpiredAt < DateTime.Now)
                .ToListAsync(stoppingToken);

            if (expiredTokens.Any())
            {
                context.RefreshTokens.RemoveRange(expiredTokens);
                await context.SaveChangesAsync(stoppingToken);
                logger.LogInformation("🧹 Deleted {Count} expired refresh tokens", expiredTokens.Count);
            }

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken); // chạy 2 lần/ngày
        }
    }
}
