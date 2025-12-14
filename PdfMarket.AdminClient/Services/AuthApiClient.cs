using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PdfMarket.Contracts.Auth;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// HTTP client wrapper for authentication endpoints.
/// Used by the AdminClient to obtain a JWT token via login.
/// </summary>
public class AuthApiClient
{
    private readonly HttpClient http;

    /// <summary>
    /// Creates a new <see cref="AuthApiClient"/> using a configured <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="http">HTTP client with BaseAddress set.</param>
    public AuthApiClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>
    /// Logs in a user and returns an <see cref="AuthResponse"/> containing JWT token and user info.
    /// </summary>
    /// <param name="userNameOrEmail">Username or email.</param>
    /// <param name="password">Plain text password (sent over HTTPS in real systems).</param>
    /// <returns>
    /// The <see cref="AuthResponse"/> if success; otherwise <c>null</c>.
    /// </returns>
    public async Task<AuthResponse?> LoginAsync(string userNameOrEmail, string password)
    {
        // DTO creation keeps API contract explicit and testable.
        var request = new LoginRequest(userNameOrEmail, password);

        // PostAsJsonAsync serializes request into JSON and posts it to the endpoint.
        var response = await http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return null;

        // Deserialize response body to AuthResponse DTO.
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
}
