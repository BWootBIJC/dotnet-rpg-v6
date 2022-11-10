using dotnet_rpg.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/{controller}")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // public Task<ServiceResponse<int>> ListUsers()
    // {
    //     try
    //     {
    //         _characterService.
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
}