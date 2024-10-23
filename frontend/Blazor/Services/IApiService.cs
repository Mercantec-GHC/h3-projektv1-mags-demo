public interface IApiService
{
    Task<HttpResponseMessage> GetAsync(string url);
    Task<HttpResponseMessage> PostAsync<T>(string url, T data);
    Task<HttpResponseMessage> PutAsync<T>(string url, T data);
    Task<HttpResponseMessage> DeleteAsync(string url);
}

