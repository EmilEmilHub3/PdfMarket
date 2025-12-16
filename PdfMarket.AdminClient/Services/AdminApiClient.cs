using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// Thin HTTP client wrapper for admin endpoints in the PdfMarket Web API.
///
/// Responsibilities:
/// - Centralizes all admin-related HTTP calls in one place.
/// - Keeps ViewModels free from raw HttpClient usage and serialization details.
/// - Provides consistent error handling so failures are visible (useful for exam/demo).
///
/// Notes:
/// - The provided <see cref="HttpClient"/> is expected to be preconfigured with:
///   - BaseAddress (e.g. http://localhost:8080/)
///   - Authorization header (Bearer JWT) when calling protected admin endpoints.
/// </summary>
public class AdminApiClient
{
    private readonly HttpClient http;

    /// <summary>
    /// Shared JSON options.
    /// Case-insensitive helps when DTO casing differs from backend JSON casing.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Creates a new <see cref="AdminApiClient"/> using a configured <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="http">
    /// Configured HTTP client with BaseAddress and optional Authorization header.
    /// </param>
    public AdminApiClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>
    /// Fetches platform statistics (total users, total PDFs, etc.) from the admin API.
    /// </summary>
    /// <returns>The stats DTO if successful.</returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the server responds with a non-success status code.
    /// </exception>
    public async Task<PlatformStatsDto?> GetStatsAsync()
    {
        // Relative URL -> resolved against HttpClient.BaseAddress
        var response = await http.GetAsync("api/admin/stats");

        if (!response.IsSuccessStatusCode)
        {
            // Read body to surface useful error details in the UI/logs
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"GetStats failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}"
            );
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlatformStatsDto>(json, JsonOptions);
    }

    /// <summary>
    /// Fetches a list of users for the admin UI.
    /// </summary>
    /// <returns>A read-only collection of users.</returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the server responds with a non-success status code.
    /// </exception>
    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
    {
        var response = await http.GetAsync("api/admin/users");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"GetUsers failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}"
            );
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<UserSummaryDto>>(json, JsonOptions)
               ?? new List<UserSummaryDto>();
    }

    /// <summary>
    /// Fetches a list of PDFs for moderation in the admin UI.
    /// </summary>
    /// <returns>A read-only collection of PDFs.</returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the server responds with a non-success status code.
    /// </exception>
    public async Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync()
    {
        var response = await http.GetAsync("api/admin/pdfs");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"GetPdfs failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}"
            );
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<AdminPdfListItemDto>>(json, JsonOptions)
               ?? new List<AdminPdfListItemDto>();
    }

    /// <summary>
    /// Deletes (or deactivates) a PDF using the admin API.
    /// </summary>
    /// <param name="pdfId">The PDF identifier.</param>
    /// <returns><c>true</c> if the server responded with success; otherwise <c>false</c>.</returns>
    public async Task<bool> DeletePdfAsync(string pdfId)
    {
        // DELETE should not require a response body; status code is enough
        var response = await http.DeleteAsync($"api/admin/pdfs/{pdfId}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Updates a user's editable fields (e.g. email, points balance).
    /// Null fields in <paramref name="request"/> mean "do not update that field".
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="request">Update payload.</param>
    /// <returns><c>true</c> if the request succeeded.</returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the server responds with a non-success status code.
    /// </exception>
    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        // Uses System.Net.Http.Json for JSON serialization of the request
        var response = await http.PutAsJsonAsync($"api/admin/users/{userId}", request);

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Update failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}"
        );
    }

    /// <summary>
    /// Resets a user's password through the admin
    public async Task<bool> ResetPasswordAsync(string userId, ResetPasswordRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await http.PostAsync(
            $"api/admin/users/{userId}/reset-password",
            content
        );

        return response.IsSuccessStatusCode;
    }
}