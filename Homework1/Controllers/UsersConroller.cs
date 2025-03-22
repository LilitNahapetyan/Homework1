using Homework1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Homework1.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static readonly List<User> users =
        [
            new() { Id = 1,  FirstName = "Franklin", LastName = "Carmichael", },
            new() { Id = 2,  FirstName = "Lawren", LastName = "Harris",  },
            new() { Id = 3,  FirstName = "Frank", LastName = "Johnston",  },
            new() { Id = 4,  FirstName = "Arthur", LastName = "Lismer",  },
            new() { Id = 5,  FirstName = "Frederick", LastName = "Varley",  }
        ];

        // POST: api/users
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User newUser)
        {
            if (newUser == null)
                return BadRequest("Invalid user data");

            newUser.Id = users.Count + 1;
            users.Add(newUser);

            return CreatedAtAction(nameof(CreateUser), new { id = newUser.Id }, newUser);
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "User not found" });

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;

            return Ok(user);
        }
    }
}


