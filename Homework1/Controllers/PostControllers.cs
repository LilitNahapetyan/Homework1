using Homework1.Models;
using Homework1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Homework1.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts([FromQuery] int userId, [FromQuery] string title)
        {
            var posts = await _postService.GetPostsAsync(userId, title);
            if (!posts.Any())
                return NoContent();

            return Ok(posts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postService.GetPostAsync(id);
            if (post == null)
                return NotFound(new { message = "Post not found" });

            return Ok(post);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var success = await _postService.DeletePostAsync(id);
            if (!success)
                return StatusCode(500, new { message = "Unexpected error during delete" });

            return NoContent();
        }
    }
}