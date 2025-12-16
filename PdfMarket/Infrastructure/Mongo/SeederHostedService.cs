using PdfMarket.Infrastructure.Mongo;

public class SeederHostedService : IHostedService
{
    private readonly MongoSeeder _seeder;
    private readonly ILogger<SeederHostedService> _logger;

    public SeederHostedService(
        MongoSeeder seeder,
        ILogger<SeederHostedService> logger)
    {
        _seeder = seeder;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MongoDB seeding...");

        const int maxRetries = 10;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await _seeder.SeedAsync();
                _logger.LogInformation("MongoDB seeding completed");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Seeding failed (attempt {Attempt}/{Max}). Retrying...",
                    attempt,
                    maxRetries
                );

                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }

        _logger.LogError("MongoDB seeding failed after maximum retries");
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
