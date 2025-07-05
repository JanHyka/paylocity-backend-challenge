using ApiTests.Infrastructure;
using System;
using System.Net.Http;

namespace ApiTests;

/// <summary>
/// Provides a base class for integration tests, managing the test server and HTTP client lifecycle.
/// </summary>
public abstract class IntegrationTest : IDisposable
{
    private HttpClient? _httpClient;

    private readonly TestWebApplicationFactory _factory = new();

    /// <summary>
    /// Gets an <see cref="HttpClient"/> instance configured for integration testing.
    /// </summary>
    protected HttpClient HttpClient
    {
        get
        {
            if (_httpClient == default)
            {
                _httpClient = _factory.CreateClient();
                _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            }

            return _httpClient;
        }
    }

    /// <summary>
    /// Disposes resources used by the <see cref="IntegrationTest"/> class.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();

        GC.SuppressFinalize(this);
    }
}

