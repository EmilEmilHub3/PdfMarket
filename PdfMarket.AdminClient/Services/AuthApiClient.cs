using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PdfMarket.Contracts.Auth;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// HTTP client wrapper for authentication endpoints.
/// Used to obtain JWT tokens via login.
/// </summary>
public class AuthApiClient
{
    private readonly HttpClient http;

    /// <summary>
    /// Creates a new AuthApiClient.
    /// </summary>
    public AuthApiClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>
    /// Performs user login and returns authentication data.
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(string userNameOrEmail, string password)
    {
        var request = new LoginRequest(userNameOrEmail, password);

        var response = await http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
}
