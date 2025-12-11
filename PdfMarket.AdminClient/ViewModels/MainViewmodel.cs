using PdfMarket.AdminClient.Services;

namespace PdfMarket.AdminClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => currentViewModel;
        set { currentViewModel = value; OnPropertyChanged(); }
    }

    public MainViewModel(AdminApiClient adminApi)
    {
        CurrentViewModel = new StatsViewModel(adminApi);
    }
}
