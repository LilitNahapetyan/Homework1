using Homework1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Homework1.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions;

        public UsersController(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IOptions<JsonOptions> jsonOptions)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = jsonOptions.Value.PropertyNameCaseInsensitive,
                WriteIndented = jsonOptions.Value.WriteIndented
            };
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.ReqresBaseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound(new { message = "User not found" });

            var userWrapper = await DeserializeResponse<UserResponse>(response);
            var user = userWrapper?.Data;

            if (user == null)
                return StatusCode(500, new { message = "Error deserializing user data" });

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
        {
            if (newUser == null)
                return BadRequest("Invalid user data");

            var response = await _httpClient.PostAsync(_apiSettings.ReqresBaseUrl, SerializeContent(newUser));
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { message = "Error creating user" });

            var createdUser = await DeserializeResponse<User>(response);
            if (createdUser == null)
                return StatusCode(500, new { message = "Error deserializing created user data" });

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var response = await _httpClient.PutAsync($"{_apiSettings.ReqresBaseUrl}/{id}", SerializeContent(updatedUser));
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { message = "Error updating user" });

            var userWrapper = await DeserializeResponse<UserResponse>(response);
            var user = userWrapper?.Data;
            if (user == null)
                return StatusCode(500, new { message = "Error deserializing updated user data" });

            return Ok(user);
        }

        private StringContent SerializeContent<T>(T obj) =>
            new(JsonSerializer.Serialize(obj, _jsonOptions), Encoding.UTF8, "application/json");

        private async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
    }
}
