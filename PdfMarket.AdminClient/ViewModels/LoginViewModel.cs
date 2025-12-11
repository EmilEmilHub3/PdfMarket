using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using PdfMarket.AdminClient.Infrastructure;
using PdfMarket.AdminClient.Services;

namespace PdfMarket.AdminClient.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly string baseUrl;
    private readonly System.Action<HttpClient> onLoginSuccess;

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
        set { isBusy = value; OnPropertyChanged(); }
    }

    public RelayCommand LoginCommand { get; }

    public LoginViewModel(string baseUrl, System.Action<HttpClient> onLoginSuccess)
    {
        this.baseUrl = baseUrl;
        this.onLoginSuccess = onLoginSuccess;

        LoginCommand = new RelayCommand(async () => await LoginAsync(),
                                        () => !IsBusy);
    }

    private async Task LoginAsync()
    {
        IsBusy = true;
        try
        {
            var http = ApiClientFactory.Create(baseUrl, token: null);
            var authApi = new AuthApiClient(http);

            var auth = await authApi.LoginAsync(UserNameOrEmail, Password);
            if (auth is null || auth.Role != "Admin")
            {
                MessageBox.Show("Login failed or not admin.");
                return;
            }

            var authedClient = ApiClientFactory.Create(baseUrl, auth.Token);
            onLoginSuccess(authedClient);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
