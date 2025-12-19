using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PdfMarket.AdminClient.Services;

/// <summary>
/// Factory responsible for creating configured HttpClient instances.
/// Centralizes base URL handling and JWT authentication setup.
/// </summary>
public static class ApiClientFactory
{
    /// <summary>
    /// Creates a configured HttpClient instance.
    /// </summary>
    /// <param name="baseUrl">Base URL of the API.</param>
    /// <param name="token">Optional JWT token.</param>
    public static HttpClient Create(string baseUrl, string? token)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("BaseUrl must be provided", nameof(baseUrl));

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
