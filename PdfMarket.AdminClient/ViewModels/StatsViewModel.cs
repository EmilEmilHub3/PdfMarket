using System.Threading.Tasks;
using PdfMarket.AdminClient.Services;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.ViewModels;

public class StatsViewModel : ViewModelBase
{
    private readonly StatsLoader statsLoader;

    private PlatformStatsDto? stats;
    public PlatformStatsDto? Stats
    {
        get => stats;
        set { stats = value; OnPropertyChanged(); }
    }

    private int progress;
    public int Progress
    {
        get => progress;
        set { progress = value; OnPropertyChanged(); }
    }

    public StatsViewModel(AdminApiClient adminApi)
    {
        statsLoader = new StatsLoader(adminApi);
    }

    public async Task LoadAsync()
    {
        Stats = await statsLoader.LoadStatsAsync(p => Progress = p);
    }
}
