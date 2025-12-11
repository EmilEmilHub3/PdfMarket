using System.Net.Http;
using System.Windows;
using PdfMarket.AdminClient.Services;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

public partial class MainWindow : Window
{
    public MainWindow(HttpClient httpClient)
    {
        InitializeComponent();

        var adminApi = new AdminApiClient(httpClient);
        DataContext = new MainViewModel(adminApi);
    }
}
