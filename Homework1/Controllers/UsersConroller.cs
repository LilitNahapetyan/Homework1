using Homework1.Models;
using Homework1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Homework1.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
        {
            if (newUser == null)
                return BadRequest("Invalid user data");

            try
            {
                var createdUser = await _userService.CreateUserAsync(newUser);
                if (createdUser == null)
                    return StatusCode(500, new { message = "Error creating user" });

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (DuplicateUserNameException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _userService.UpdateUserAsync(id, updatedUser);
            if (user == null)
                return StatusCode(500, new { message = "Error updating user" });

            return Ok(user);
        }
    }

}
