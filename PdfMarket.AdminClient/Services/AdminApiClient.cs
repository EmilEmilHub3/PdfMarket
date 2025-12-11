using System.Collections.Generic;
using System.Net.Http;
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
}
