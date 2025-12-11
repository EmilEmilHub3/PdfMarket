using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PdfMarket.AdminClient.Services;

// Factory pattern: samler oprettelse af HttpClient
public static class ApiClientFactory
{
    public static HttpClient Create(string baseUrl, string? token)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return http;
    }
}
