using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// Interaction logic for LoginWindow.
/// 
/// Responsibilities:
/// - Hosts the LoginViewModel
/// - Handles PasswordBox input (cannot be bound directly in WPF)
/// - Navigates to MainWindow on successful login
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel viewModel;

    public LoginWindow()
    {
        InitializeComponent();

        
        string baseUrl = "https://localhost:7268";

        viewModel = new LoginViewModel(baseUrl, OnLoginSuccess);
        DataContext = viewModel;
    }

    /// <summary>
    /// Callback invoked by the LoginViewModel when login succeeds.
    /// Opens the main admin window and closes the login window.
    /// </summary>
    private void OnLoginSuccess(HttpClient httpClient)
    {
        var main = new MainWindow(httpClient);
        main.Show();
        Close();
    }

    /// <summary>
    /// Manually transfers password input from PasswordBox to the ViewModel.
    /// Required because PasswordBox does not support data binding.
    /// </summary>
    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
        {
            vm.Password = pb.Password;
        }
    }
}
