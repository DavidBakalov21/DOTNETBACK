
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
    [Authorize(AuthenticationSchemes = "Access", Roles = "admin")]
    public async Task<IActionResult> GetUsers(int page)
    {
        var testData = await _userService.GetUsers(page);
        return Ok(testData);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleteStatus = await _userService.DeleteUser(id);
        if (deleteStatus == false)
        {
            return NotFound(); 
        } 
        return NoContent();
    }
    
}