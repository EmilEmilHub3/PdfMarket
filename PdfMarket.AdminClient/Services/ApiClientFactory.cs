using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// Factory for creating configured <see cref="HttpClient"/> instances for the AdminClient.
/// 
/// Why a factory?
/// - Centralizes BaseAddress + Authorization header setup.
/// - Keeps construction logic out of ViewModels and services.
/// </summary>
public static class ApiClientFactory
{
    /// <summary>
    /// Creates an <see cref="HttpClient"/> configured with base URL and optional JWT Bearer token.
    /// </summary>
    /// <param name="baseUrl">API base URL, e.g. "http://localhost:8080/".</param>
    /// <param name="token">JWT token (optional). If provided, adds Authorization: Bearer header.</param>
    /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
    public static HttpClient Create(string baseUrl, string? token)
    {
        var http = new HttpClient
        {
            // BaseAddress ensures relative URLs like "api/admin/stats" work.
            BaseAddress = new Uri(baseUrl)
        };

        // Only set the header when a token is present.
        if (!string.IsNullOrWhiteSpace(token))
        {
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return http;
    }
}
