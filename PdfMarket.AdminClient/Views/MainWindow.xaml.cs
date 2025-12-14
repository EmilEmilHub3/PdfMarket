using System.Net.Http;
using System.Windows;
using PdfMarket.AdminClient.Services;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// Main application window for the admin client.
/// 
/// Hosts the MainViewModel which controls navigation
/// between Stats, PDF moderation and User management.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(HttpClient httpClient)
    {
        InitializeComponent();

        // Create API client using authenticated HttpClient
        var adminApi = new AdminApiClient(httpClient);

        DataContext = new MainViewModel(adminApi);
    }
}
