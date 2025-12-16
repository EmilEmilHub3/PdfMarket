using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// Factory for creating configured <see cref="HttpClient"/> instances for the AdminClient.
///
/// Centralizes:
/// - API base address handling
/// - JWT Authorization header setup
/// </summary>
public static class ApiClientFactory
{
    public static HttpClient Create(string baseUrl, string? token)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("BaseUrl must be provided", nameof(baseUrl));

        // Ensure trailing slash so relative URLs work correctly
        if (!baseUrl.EndsWith("/"))
            baseUrl += "/";

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
