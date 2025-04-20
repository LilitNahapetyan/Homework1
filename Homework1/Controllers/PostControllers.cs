using Homework1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Homework1.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions;

        public PostsController(HttpClient httpClient, IOptions<ApiSettings> apiSettings, JsonSerializerOptions jsonSerializerOptions)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _jsonOptions = jsonSerializerOptions;
        }

        // GET: api/posts?userId=1&title=understanding%20the%20basics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts([FromQuery] int userId, [FromQuery] string title)
        {
            var url = $"{_apiSettings.JsonPlaceholderBaseUrl}?userId={userId}&title={Uri.EscapeDataString(title)}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return NotFound(new { message = "Posts not found" });

            var content = await response.Content.ReadAsStringAsync();
            var filteredPosts = JsonSerializer.Deserialize<List<Post>>(content, _jsonOptions);

            if (filteredPosts == null || filteredPosts.Count == 0)
            {
                return NoContent();
            }

            return Ok(filteredPosts);
        }

        // GET: api/posts/3
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var url = $"{_apiSettings.JsonPlaceholderBaseUrl}/{id}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return NotFound(new { message = "Post not found" });

            var content = await response.Content.ReadAsStringAsync();
            var post = JsonSerializer.Deserialize<Post>(content, _jsonOptions);

            return Ok(post);
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var url = $"{_apiSettings.JsonPlaceholderBaseUrl}/{id}";
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
                return NoContent();

            return StatusCode((int)response.StatusCode, new { message = "Unexpected error during delete" });
        }
    }
}

