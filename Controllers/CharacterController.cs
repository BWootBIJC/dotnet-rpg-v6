using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    private static List<Character> characters = new List<Character>
    {
        new Character(),
        new Character{ Id = 1, Name = "Sam" }
    };

    [HttpGet]
    [Route("GetAll")]
    public ActionResult<List<Character>> Get()
    {
        return Ok(characters);
    }
    
    [HttpGet]
    [Route("{id}")]
    public ActionResult<Character> GetSingle(int id)
    {
        return Ok(characters.FirstOrDefault(c => c.Id == id));
    }
}