using System.Security.Claims;
using DotNetAdditional.DTO;
using DotNetAdditional.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAdditional.Controller;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        var (at, rt) = await _authService.Login(request.Email, request.Password);
        return Ok(new { AccessToken = at, RefreshToken = rt });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        var (at, rt) =await _authService.Register(request.Username, request.Password, request.Email);
        Console.WriteLine(request.Email);
        Console.WriteLine(request.Username);
        Console.WriteLine(request.Password);
        return Ok(new { AccessToken = at, RefreshToken = rt });
    }
    [HttpGet("refresh")]
    [Authorize(AuthenticationSchemes = "Refresh")]
    public async Task<IActionResult> Refresh()
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var (at, rt) =await _authService.Refresh(userEmail);
        return Ok(new { AccessToken = at, RefreshToken = rt });
    }
    
}