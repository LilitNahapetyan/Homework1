using Homework1.Models;
using Microsoft.AspNetCore.Mvc;


namespace Homework1.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private static readonly List<Post> posts = new()
        {
            new() { UserId = 1, Id = 1, Title = "qui est esse" ,Body = "est rerum tempore vitae sequi sint nihil reprehenderit dolor beatae ea dolores neque fugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis qui aperiam non debitis possimus qui neque nisi nulla"  },
            new() { UserId = 2, Id= 11, Title = "et ea vero quia laudantium autem", Body = "delectus reiciendis molestiae occaecati non minima eveniet qui voluptatibus accusamus in eum beatae sit vel qui neque voluptates ut commodi qui incidunt ut animi commodi"        },
            new() { UserId = 3, Id= 22, Title= "dolor sint quo a velit explicabo quia nam",Body= "eos qui et ipsum ipsam suscipit aut sed omnis non odio expedita earum mollitia molestiae aut atque rem suscipit nam impedit esse"}
        };

        // GET: api/posts?userId=1&title=understanding%20the%20basics
        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetPosts([FromQuery] int userId, [FromQuery] string title)
        {
            var filteredPosts = posts
                .Where(p => p.UserId == userId && p.Title.ToLower().Contains(title.ToLower()))
                .ToList();

            if (filteredPosts.Count == 0)
            {
                return NoContent();
            }

            return Ok(filteredPosts);
        }

        // GET: api/posts/3
        [HttpGet("{id:int}")]
        public ActionResult<Post> GetPost(int id)
        {
            var post = posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound(new { message = "Post not found" });

            return Ok(post);
        }

        // DELETE: api/posts/3
        [HttpDelete("{id:int}")]
        public ActionResult DeletePost(int id)
        {
            var post = posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound(new { message = "Post not found" });

            posts.Remove(post);
            return Ok(new { message = "Post deleted successfully" });
        }
    }
}

