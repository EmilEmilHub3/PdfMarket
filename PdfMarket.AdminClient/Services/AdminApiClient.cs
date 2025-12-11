using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.Services;

public class AdminApiClient
{
    private readonly HttpClient http;

    public AdminApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<PlatformStatsDto?> GetStatsAsync()
        => await http.GetFromJsonAsync<PlatformStatsDto>("api/admin/stats");

    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync()
        => await http.GetFromJsonAsync<IReadOnlyCollection<UserSummaryDto>>("api/admin/users")
           ?? new List<UserSummaryDto>();
}
