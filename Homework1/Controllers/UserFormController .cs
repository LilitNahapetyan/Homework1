using Homework1.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Homework1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFormController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetForm() => Ok(UserFormStorage.Current);

        [HttpPatch("patch")]
        public IActionResult Patch([FromBody] JsonPatchDocument<UserFormModel> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest("Patch document is null.");

            var model = UserFormStorage.Current;

            var modelCopy = new UserFormModel
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                DateOfBirth = model.DateOfBirth,
                Quantity = model.Quantity,
                Price = model.Price,
                Amount = model.Amount
            };

            patchDoc.ApplyTo(modelCopy, ModelState);

            var context = new ValidationContext(modelCopy);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(modelCopy, context, results, true);

            if (!isValid || !ModelState.IsValid)
            {
                foreach (var result in results)
                {
                    if (!string.IsNullOrEmpty(result.ErrorMessage)) 
                    {
                        ModelState.AddModelError(string.Empty, errorMessage: result.ErrorMessage);
                    }
                }
                return ValidationProblem(ModelState);
            }

            UserFormStorage.Current = modelCopy;
            return Ok(UserFormStorage.Current);
        }



    }
}
