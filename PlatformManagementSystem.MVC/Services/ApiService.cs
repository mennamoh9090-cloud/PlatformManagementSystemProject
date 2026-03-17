using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient CreateClientWithToken()
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = _httpContextAccessor
            .HttpContext?
            .Session
            .GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var client = CreateClientWithToken();

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return default;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<bool> PostAsync<T>(string url, T data)
    {
        var client = CreateClientWithToken();

        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(url, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PutAsync<T>(string url, T data)
    {
        var client = CreateClientWithToken();

        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync(url, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string url)
    {
        var client = CreateClientWithToken();

        var response = await client.DeleteAsync(url);

        return response.IsSuccessStatusCode;
    }

}