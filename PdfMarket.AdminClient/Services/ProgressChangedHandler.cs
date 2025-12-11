using System;
using System.Threading.Tasks;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.Services;

// Delegate til progress (exam: delegates + MT)
public delegate void ProgressChangedHandler(int percent);

public class StatsLoader
{
    private readonly AdminApiClient adminApi;

    public StatsLoader(AdminApiClient adminApi)
    {
        this.adminApi = adminApi;
    }

    public async Task<PlatformStatsDto?> LoadStatsAsync(ProgressChangedHandler onProgress)
    {
        // Simuler lidt progress
        for (int i = 0; i <= 80; i += 20)
        {
            await Task.Delay(100); // kører ikke på UI-tråden
            onProgress(i);
        }

        var stats = await adminApi.GetStatsAsync();
        onProgress(100);

        return stats;
    }
}
