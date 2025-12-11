using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PdfMarket.Contracts.Auth;

namespace PdfMarket.AdminClient.Services;

public class AuthApiClient
{
    private readonly HttpClient http;

    public AuthApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<AuthResponse?> LoginAsync(string userNameOrEmail, string password)
    {
        var request = new LoginRequest(userNameOrEmail, password);

        var response = await http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
}
