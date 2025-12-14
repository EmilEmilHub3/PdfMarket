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
/// Purpose:
/// - Centralizes all admin-related HTTP calls in one place.
/// - Keeps ViewModels/UI free from raw HttpClient usage and serialization details.
/// </summary>
public class AdminApiClient
{
    private readonly HttpClient http;

    /// <summary>
    /// Shared JSON options for manual serialization/deserialization.
    /// Case-insensitive helps when backend uses different casing than the client DTO properties.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Creates a new <see cref="AdminApiClient"/> using an already-configured <see cref="HttpClient"/>.
    /// The HttpClient is expected to have BaseAddress + Authorization header set by the caller/factory.
    /// </summary>
    /// <param name="http">Configured HTTP client (BaseAddress + optional Bearer token).</param>
    public AdminApiClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>
    /// Fetches platform statistics (total users, total pdfs, etc.) from the admin API.
    /// </summary>
    /// <returns>A <see cref="PlatformStatsDto"/> if successful; otherwise <c>null</c>.</returns>
    public async Task<PlatformStatsDto?> GetStatsAsync()
    {
        var response = await http.GetAsync("api/admin/stats");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlatformStatsDto>(json, JsonOptions);
    }

    /// <summary>
    /// Fetches a list of users for the admin UI.
    /// </summary>
    /// <returns>A read-only collection of users. Returns an empty list on failure.</returns>
    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
    {
        var response = await http.GetAsync("api/admin/users");
        if (!response.IsSuccessStatusCode)
            return new List<UserSummaryDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<UserSummaryDto>>(json, JsonOptions)
               ?? new List<UserSummaryDto>();
    }

    /// <summary>
    /// Fetches a list of PDFs for the admin UI.
    /// </summary>
    /// <returns>A read-only collection of PDFs. Returns an empty list on failure.</returns>
    public async Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync()
    {
        var response = await http.GetAsync("api/admin/pdfs");
        if (!response.IsSuccessStatusCode)
            return new List<AdminPdfListItemDto>();

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
        var response = await http.DeleteAsync($"api/admin/pdfs/{pdfId}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Updates a user's editable fields (e.g. email, points balance).
    /// 
    /// NOTE:
    /// - Throws on non-success responses so the ViewModel can show a clear error message.
    /// - The exception includes status code + response body to make debugging easy during exam/demo.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="request">Update payload (nullable fields mean "only update what is set").</param>
    /// <returns><c>true</c> if the request succeeded.</returns>
    /// <exception cref="HttpRequestException">Thrown when the server returns a non-success status code.</exception>
    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var response = await http.PutAsJsonAsync($"/api/admin/users/{userId}", request);

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();

        throw new HttpRequestException(
            $"Update failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}"
        );
    }

    /// <summary>
    /// Resets a user's password through the admin API.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="request">Contains the new password.</param>
    /// <returns><c>true</c> if the server responded with success; otherwise <c>false</c>.</returns>
    public async Task<bool> ResetPasswordAsync(string userId, ResetPasswordRequest request)
    {
        // Manual serialization is kept here to show explicit JSON handling 
        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await http.PostAsync($"api/admin/users/{userId}/reset-password", content);
        return response.IsSuccessStatusCode;
    }
}
