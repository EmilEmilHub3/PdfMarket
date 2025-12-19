using System.Net.Http;
using System.Windows;
using PdfMarket.AdminClient.Services;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// Main application window for the admin client.
///
/// Hosts the MainViewModel which controls navigation
/// between statistics, PDF moderation, and user management.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes the main window using an authenticated HttpClient.
    /// Creates the AdminApiClient and assigns the MainViewModel
    /// as the DataContext.
    /// </summary>
    /// <param name="httpClient">Authenticated HttpClient instance.</param>
    public MainWindow(HttpClient httpClient)
    {
        InitializeComponent();

        // Create admin API client using authenticated HttpClient
        var adminApi = new AdminApiClient(httpClient);

        DataContext = new MainViewModel(adminApi);
    }
}
