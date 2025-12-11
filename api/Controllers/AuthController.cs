using System.Threading.Tasks;
using api.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login/student")]
    public async Task<IActionResult> LoginStudent([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginStudentAsync(request.IdOrUsername, request.Password);
        if (token == null)
        {
            return Unauthorized("Invalid Student ID or Password");
        }
        return Ok(new { Token = token });
    }

    [HttpPost("login/admin")]
    public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAdminAsync(request.IdOrUsername, request.Password);
        if (token == null)
        {
            return Unauthorized("Invalid Username or Password");
        }
        return Ok(new { Token = token });
    }
}

public class LoginRequest
{
    public string IdOrUsername { get; set; } = null!;
    public string Password { get; set; } = null!;
}
