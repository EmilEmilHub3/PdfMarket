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

    /// <summary>
    /// Initializes the LoginViewModel.
    /// </summary>
    /// <param name="baseUrl">Base URL of the backend API.</param>
    /// <param name="onLoginSuccess">
    /// Callback executed when login succeeds.
    /// Receives an authenticated HttpClient.
    /// </param>
    public LoginViewModel(string baseUrl, Action<HttpClient> onLoginSuccess)
    {
        this.baseUrl = baseUrl;
        this.onLoginSuccess = onLoginSuccess;

        LoginCommand = new RelayCommand(async () => await LoginAsync(), () => !IsBusy);
    }

    /// <summary>
    /// Performs admin login against the API.
    /// Validates credentials, checks admin role,
    /// and initializes an authenticated HttpClient.
    /// </summary>
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
                MessageBox.Show(ErrorMessage, "Login failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var authedClient = ApiClientFactory.Create(baseUrl, auth.Token);
            onLoginSuccess(authedClient);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            MessageBox.Show($"Login error:\n\n{ex.Message}",
                "Login error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

}
