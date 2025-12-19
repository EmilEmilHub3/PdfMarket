using PdfMarket.AdminClient.Infrastructure;
using PdfMarket.AdminClient.Services;

namespace PdfMarket.AdminClient.ViewModels;

/// <summary>
/// Root ViewModel that coordinates navigation between
/// different admin sections (Stats, PDFs, Users).
/// </summary>
public class MainViewModel : ViewModelBase
{
    public StatsViewModel StatsViewModel { get; }
    public PdfModerationViewModel PdfModerationViewModel { get; }
    public UsersViewModel UsersViewModel { get; }

    private ViewModelBase currentViewModel = null!;
    public ViewModelBase CurrentViewModel
    {
        get => currentViewModel;
        set
        {
            currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand ShowStatsCommand { get; }
    public RelayCommand ShowPdfsCommand { get; }
    public RelayCommand ShowUsersCommand { get; }

    /// <summary>
    /// Initializes the MainViewModel and all child ViewModels.
    /// </summary>
    /// <param name="adminApi">Authenticated admin API client.</param>
    public MainViewModel(AdminApiClient adminApi)
    {
        StatsViewModel = new StatsViewModel(adminApi);
        PdfModerationViewModel = new PdfModerationViewModel(adminApi);
        UsersViewModel = new UsersViewModel(adminApi);

        CurrentViewModel = StatsViewModel;

        ShowStatsCommand = new RelayCommand(async () =>
        {
            CurrentViewModel = StatsViewModel;
            await StatsViewModel.LoadAsync();
        });

        ShowPdfsCommand = new RelayCommand(async () =>
        {
            CurrentViewModel = PdfModerationViewModel;
            await PdfModerationViewModel.LoadAsync();
        });

        ShowUsersCommand = new RelayCommand(async () =>
        {
            CurrentViewModel = UsersViewModel;
            await UsersViewModel.LoadAsync();
        });

        // Initial data load
        _ = StatsViewModel.LoadAsync();
    }

}
