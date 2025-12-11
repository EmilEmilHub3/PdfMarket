using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using PdfMarket.AdminClient.Services;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel viewModel;

    public LoginWindow()
    {
        InitializeComponent();

        // RET baseUrl hvis din API kører på en anden adresse
        string baseUrl = "https://localhost:7268";

        viewModel = new LoginViewModel(baseUrl, OnLoginSuccess);
        DataContext = viewModel;
    }

    private void OnLoginSuccess(HttpClient httpClient)
    {
        // Åbn MainWindow og luk login
        var main = new MainWindow(httpClient);
        main.Show();
        Close();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
        {
            vm.Password = pb.Password;
        }
    }
}
