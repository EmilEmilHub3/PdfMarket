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
/// - Centralizes all admin-related HTTP calls.
/// - Keeps ViewModels free from HttpClient and serialization logic.
/// - Surfaces HTTP errors clearly for UI feedback and exam demonstration.
/// </summary>
public class AdminApiClient
{
    private readonly HttpClient http;

    /// <summary>
    /// Shared JSON serializer options.
    /// Case-insensitive mapping avoids casing issues between backend and DTOs.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Creates a new AdminApiClient using a configured HttpClient.
    /// </summary>
    /// <param name="http">
    /// HttpClient with BaseAddress set and optional Authorization header.
    /// </param>
    public AdminApiClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>
    /// Retrieves platform statistics from the admin API.
    /// </summary>
    /// <returns>Platform statistics DTO.</returns>
    public async Task<PlatformStatsDto?> GetStatsAsync()
    {
        var response = await http.GetAsync("api/admin/stats");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"GetStats failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlatformStatsDto>(json, JsonOptions);
    }

    /// <summary>
    /// Retrieves all users for administration.
    /// </summary>
    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
    {
        var response = await http.GetAsync("api/admin/users");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"GetUsers failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<UserSummaryDto>>(json, JsonOptions)
               ?? new List<UserSummaryDto>();
    }

    /// <summary>
    /// Retrieves all PDFs for moderation.
    /// </summary>
    public async Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync()
    {
        var response = await http.GetAsync("api/admin/pdfs");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"GetPdfs failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<AdminPdfListItemDto>>(json, JsonOptions)
               ?? new List<AdminPdfListItemDto>();
    }

    /// <summary>
    /// Deletes a PDF using the admin API.
    /// </summary>
    public async Task<bool> DeletePdfAsync(string pdfId)
    {
        var response = await http.DeleteAsync($"api/admin/pdfs/{pdfId}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Updates a user's editable fields.
    /// </summary>
    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/admin/users/{userId}", request);

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Update failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
    }

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    public async Task<bool> ResetPasswordAsync(string userId, ResetPasswordRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await http.PostAsync(
            $"api/admin/users/{userId}/reset-password",
            content);

        return response.IsSuccessStatusCode;
    }
}
