using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;

public abstract class BaseApiService : IApiService
{
    protected readonly HttpClient _httpClient;
    protected readonly ILocalStorageService _localStorage;

    public BaseApiService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    protected async Task SetAuthHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        await SetAuthHeader();
        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {
        await SetAuthHeader();
        return await _httpClient.PostAsJsonAsync(url, data);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        await SetAuthHeader();
        return await _httpClient.PutAsJsonAsync(url, data);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        await SetAuthHeader();
        return await _httpClient.DeleteAsync(url);
    }
}
