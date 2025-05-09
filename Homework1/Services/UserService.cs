using Homework1.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Homework1.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> CreateUserAsync(User newUser);
        Task<User?> UpdateUserAsync(int id, User updatedUser);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _jsonOptions = jsonOptions;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var url = $"{_apiSettings.ReqresBaseUrl}{id}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<UserResponse>(content, _jsonOptions);
            return wrapper?.Data;
        }

        public async Task<User?> CreateUserAsync(User newUser)
        {
            var existingUsersResponse = await _httpClient.GetAsync(_apiSettings.ReqresBaseUrl);
            if (!existingUsersResponse.IsSuccessStatusCode)
                return null;

            var content = await existingUsersResponse.Content.ReadAsStringAsync();
            var usersWrapper = JsonSerializer.Deserialize<UsersResponse>(content, _jsonOptions);
            var existingUsers = usersWrapper?.Data;

            if (existingUsers != null && existingUsers.Any(u =>
                string.Equals(u.First_Name, newUser.First_Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(u.Last_Name, newUser.Last_Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateUserNameException("A user with the same name already exists.");
            }

            var postContent = new StringContent(JsonSerializer.Serialize(newUser, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiSettings.ReqresBaseUrl, postContent);
            if (!response.IsSuccessStatusCode)
                return null;

            return new User
            {
                Id = 72,
                First_Name = newUser.First_Name,
                Last_Name = newUser.Last_Name,
                Email = newUser.Email,
                Avatar = newUser.Avatar
            };
        }

        public async Task<User?> UpdateUserAsync(int id, User updatedUser)
        {
            var putContent = new StringContent(JsonSerializer.Serialize(updatedUser, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiSettings.ReqresBaseUrl}/{id}", putContent);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(content, _jsonOptions);
        }
    }
}
