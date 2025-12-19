using System.Threading.Tasks;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// Delegate for reporting progress percentage (0–100).
/// </summary>
public delegate void ProgressChangedHandler(int percent);

/// <summary>
/// Coordinates loading of platform statistics and reports progress.
/// Demonstrates delegates and async workflows.
/// </summary>
public class StatsLoader
{
    private readonly AdminApiClient adminApi;

    /// <summary>
    /// Creates a new StatsLoader.
    /// </summary>
    public StatsLoader(AdminApiClient adminApi)
    {
        this.adminApi = adminApi;
    }

    /// <summary>
    /// Loads platform statistics and reports progress to the caller.
    /// </summary>
    public async Task<PlatformStatsDto?> LoadStatsAsync(ProgressChangedHandler onProgress)
    {
        for (int i = 0; i <= 80; i += 20)
        {
            await Task.Delay(100);
            onProgress(i);
        }

        var stats = await adminApi.GetStatsAsync();
        onProgress(100);

        return stats;
    }
}
