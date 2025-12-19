using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// Login window for the admin client.
/// Hosts the LoginViewModel and handles minimal UI-specific logic.
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel viewModel;

    /// <summary>
    /// Initializes the login window and configures
    /// the LoginViewModel with the backend API base URL.
    /// </summary>
    public LoginWindow()
    {
        InitializeComponent();

        // Docker-hosted backend API (HTTP)
        string baseUrl = "http://localhost:8080/";

        viewModel = new LoginViewModel(baseUrl, OnLoginSuccess);
        DataContext = viewModel;
    }

    /// <summary>
    /// Callback executed after a successful admin login.
    /// Opens the MainWindow using an authenticated HttpClient
    /// and closes the login window.
    /// </summary>
    /// <param name="httpClient">Authenticated HttpClient instance.</param>
    private void OnLoginSuccess(HttpClient httpClient)
    {
        var main = new MainWindow(httpClient);
        main.Show();
        Close();
    }

    /// <summary>
    /// Handles PasswordBox changes and forwards the entered password
    /// to the LoginViewModel.
    /// This is required because PasswordBox does not support
    /// direct data binding in WPF.
    /// </summary>
    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
        {
            vm.Password = pb.Password;
        }
    }
}
