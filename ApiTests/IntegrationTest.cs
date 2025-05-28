using Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiTests;

public class IntegrationTest : IDisposable
{
    public IntegrationTest()
    {
        Factory = new WebApplicationFactory<Program>();    
    }

    public WebApplicationFactory<Program> Factory { get; private set; }
    private HttpClient? _httpClient;

    protected HttpClient HttpClient
    {
        get
        {
            if (_httpClient == default)
            {
                _httpClient = Factory.CreateDefaultClient();
                _httpClient.DefaultRequestHeaders.Add("accept", "text/plain");
            }

            return _httpClient;
        }
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        Factory.Dispose();
    }
}

