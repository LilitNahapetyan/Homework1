using Homework1.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Homework1.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync(int userId, string title);
        Task<Post> GetPostAsync(int id);
        Task<bool> DeletePostAsync(int id);
    }

    public class PostService : IPostService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions;

        public PostService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, JsonSerializerOptions jsonSerializerOptions)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _jsonOptions = jsonSerializerOptions;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(int userId, string title)
        {
            var url = $"{_apiSettings.JsonPlaceholderBaseUrl}?userId={userId}&title={Uri.EscapeDataString(title)}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<Post>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Post>>(content, _jsonOptions) ?? Enumerable.Empty<Post>();
        }

        public async Task<Post?> GetPostAsync(int id)
        {
            var url = $"{_apiSettings.JsonPlaceholderBaseUrl}/{id}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Post>(content, _jsonOptions);
        }


        public async Task<bool> DeletePostAsync(int id)
        {
            var url = $"{_apiSettings.JsonPlaceholderBaseUrl}/{id}";
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound;
        }
    }


}
