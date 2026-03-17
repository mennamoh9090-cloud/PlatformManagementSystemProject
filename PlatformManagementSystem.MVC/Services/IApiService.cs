public interface IApiService
{
    Task<T?> GetAsync<T>(string url);
    Task<bool> PostAsync<T>(string url, T data);
    Task<bool> PutAsync<T>(string url, T data);
    Task<bool> DeleteAsync(string url);
}