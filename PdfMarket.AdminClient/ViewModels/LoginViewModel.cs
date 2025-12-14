using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using PdfMarket.AdminClient.Infrastructure;
using PdfMarket.AdminClient.Services;

namespace PdfMarket.AdminClient.ViewModels;

/// <summary>
/// ViewModel responsible for admin login.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private readonly string baseUrl;
    private readonly Action<HttpClient> onLoginSuccess;

    private string userNameOrEmail = "";
    public string UserNameOrEmail
    {
        get => userNameOrEmail;
        set { userNameOrEmail = value; OnPropertyChanged(); }
    }

    private string password = "";
    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }
    }

    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            isBusy = value;
            OnPropertyChanged();
            LoginCommand.RaiseCanExecuteChanged();
        }
    }

    private string? errorMessage;
    public string? ErrorMessage
    {
        get => errorMessage;
        set { errorMessage = value; OnPropertyChanged(); }
    }

    public RelayCommand LoginCommand { get; }

    public LoginViewModel(string baseUrl, Action<HttpClient> onLoginSuccess)
    {
        this.baseUrl = baseUrl;
        this.onLoginSuccess = onLoginSuccess;

        LoginCommand = new RelayCommand(async () => await LoginAsync(), () => !IsBusy);
    }

    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var http = ApiClientFactory.Create(baseUrl, token: null);
            var authApi = new AuthApiClient(http);

            var auth = await authApi.LoginAsync(UserNameOrEmail, Password);

            if (auth is null || auth.Role != "Admin")
            {
                ErrorMessage = "Login failed or user is not an admin.";
                MessageBox.Show(ErrorMessage, "Login failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var authedClient = ApiClientFactory.Create(baseUrl, auth.Token);
            onLoginSuccess(authedClient);
        }
        catch (Exception ex)
        {
            // Keep UI-friendly feedback; detailed info can still be read from ex.Message.
            ErrorMessage = ex.Message;
            MessageBox.Show($"Login error:\n\n{ex.Message}", "Login error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
