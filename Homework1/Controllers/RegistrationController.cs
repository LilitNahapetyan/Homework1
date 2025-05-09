using Homework1.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class UserFormController : ControllerBase
{
    private static List<UserFormModel> _userForms = new List<UserFormModel>
{
    new UserFormModel
    {
        Id = 1,  
        Username = "Ani85",
        Email = "ani@example.com",
        Password = "AniStrong!85",
        DateOfBirth = new DateTime(1990, 5, 15),
        Quantity = 5,
        Price = "15.50",
        Amount = 10
    },
    new UserFormModel
    {
        Id = 2,  
        Username = "Gor_2000",
        Email = "gor@example.com",
        Password = "Gor@2000pass",
        DateOfBirth = new DateTime(2000, 9, 10),
        Quantity = 20,
        Price = "100.00",
        Amount = 45
    },
    new UserFormModel
    {
        Id = 3,  
        Username = "Mariam",
        Email = "mariam@example.com",
        Password = "Mari@1234",
        DateOfBirth = new DateTime(1985, 3, 22),
        Quantity = 3,
        Price = "9.99",
        Amount = 5
    }
};


    [HttpGet("{id}")]
    public IActionResult GetForm(int id)
    {
        var model = _userForms.FirstOrDefault(f => f.Id == id);  
        if (model == null)
            return NotFound($"User form with ID {id} not found.");

        return Ok(model);
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] JsonPatchDocument<UserFormModel> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest("Patch document is null.");

        var model = _userForms.FirstOrDefault(f => f.Id == id);  
        if (model == null)
            return NotFound($"User form with ID {id} not found.");


        patchDoc.ApplyTo(model, ModelState);

        if (!TryValidateModel(model))  
        {
            return NoContent();
        }

        return Ok(model);  
    }

}

