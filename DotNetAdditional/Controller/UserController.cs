
using DotNetAdditional.Services;
using Microsoft.AspNetCore.Authorization;

namespace DotNetAdditional.Controller;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Access")]
    public IActionResult GetUsers()
    {
        var testData = _userService.GetUsers();
        return Ok(testData);
    }
    
}