using System.Threading.Tasks;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// Delegate used to report progress as a percentage (0..100).
/// This is useful to demonstrate delegates + async workflows for the oral exam.
/// </summary>
/// <param name="percent">Progress percentage, typically between 0 and 100.</param>
public delegate void ProgressChangedHandler(int percent);

/// <summary>
/// Coordinates loading of platform stats and reports progress back to the caller.
/// 
/// Design intent:
/// - Keeps the "workflow" (simulate progress + load data) separate from the ViewModel.
/// - Allows the ViewModel to bind to progress and show a progress bar / loading indicator.
/// </summary>
public class StatsLoader
{
    private readonly AdminApiClient adminApi;

    /// <summary>
    /// Creates a new loader using an <see cref="AdminApiClient"/>.
    /// </summary>
    /// <param name="adminApi">Admin API client used to fetch stats.</param>
    public StatsLoader(AdminApiClient adminApi)
    {
        this.adminApi = adminApi;
    }

    /// <summary>
    /// Loads platform stats asynchronously and reports progress through a callback.
    /// </summary>
    /// <param name="onProgress">
    /// Callback invoked with progress percentages.
    /// Typical usage: update a Progress property in the ViewModel.
    /// </param>
    /// <returns>The loaded <see cref="PlatformStatsDto"/> or <c>null</c> if API call fails.</returns>
    public async Task<PlatformStatsDto?> LoadStatsAsync(ProgressChangedHandler onProgress)
    {
        // Simulate progress to provide UI feedback even if the real API call is fast.
        // This also makes the async flow visible during a demo.
        for (int i = 0; i <= 80; i += 20)
        {
            // Delay does not block the UI thread (await yields control back to the caller).
            await Task.Delay(100);
            onProgress(i);
        }

        // Perform the actual API call.
        var stats = await adminApi.GetStatsAsync();

        // Final progress update indicates the workflow is complete (success or failure).
        onProgress(100);

        return stats;
    }
}
