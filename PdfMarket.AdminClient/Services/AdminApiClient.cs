using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.Services;

public class AdminApiClient
{
    private readonly HttpClient http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AdminApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<PlatformStatsDto?> GetStatsAsync()
    {
        var response = await http.GetAsync("api/admin/stats");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlatformStatsDto>(json, JsonOptions);
    }

    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
    {
        var response = await http.GetAsync("api/admin/users");
        if (!response.IsSuccessStatusCode)
            return new List<UserSummaryDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<UserSummaryDto>>(json, JsonOptions)
               ?? new List<UserSummaryDto>();
    }

    // -----------------------
    // US11 - PDFs moderation
    // -----------------------
    public async Task<IReadOnlyCollection<AdminPdfListItemDto>> GetPdfsAsync()
    {
        var response = await http.GetAsync("api/admin/pdfs");
        if (!response.IsSuccessStatusCode)
            return new List<AdminPdfListItemDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<AdminPdfListItemDto>>(json, JsonOptions)
               ?? new List<AdminPdfListItemDto>();
    }

    public async Task<bool> DeletePdfAsync(string pdfId)
    {
        var response = await http.DeleteAsync($"api/admin/pdfs/{pdfId}");
        return response.IsSuccessStatusCode;
    }

    // -----------------------
    // US12 - Update user info
    // -----------------------
    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // HttpClient har PatchAsync i nyere .NET. Hvis din ikke har:
        // brug HttpRequestMessage-versionen herunder.
        var response = await http.PatchAsync($"api/admin/users/{userId}", content);
        return response.IsSuccessStatusCode;
    }

    // Hvis din .NET ikke har PatchAsync, så erstat UpdateUserAsync med denne version:
    /*
    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var message = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/admin/users/{userId}")
        {
            Content = content
        };

        var response = await http.SendAsync(message);
        return response.IsSuccessStatusCode;
    }
    */
}
