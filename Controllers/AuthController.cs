using dotnet_rpg.Dtos.User;
using dotnet_rpg.Repos;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/Users")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepo)
    {
        _authRepository = authRepo;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
    {
        var response = await _authRepository.Register(
            new User { Username = request.Username }, request.Password
            );
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}