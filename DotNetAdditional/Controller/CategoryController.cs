using System.Security.Claims;
using DotNetAdditional.DTO.Category;
using DotNetAdditional.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAdditional.Controller;
[ApiController]
[Route("category")]
public class CategoryController:ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access", Roles = "admin")]

    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO request)
    {
        var status =await _categoryService.CreateCategory(request);
        if (status)
        {
            return StatusCode(200);
        }

        return StatusCode(500);
    }


    [HttpGet]
    [Authorize(AuthenticationSchemes = "Access", Roles = "admin")]
    public async Task<IActionResult> GetCategories(int page)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var post = await _categoryService.GetCategory(page);
        return Ok(post);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleteStatus = await _categoryService.DeleteCategory(id);
        if (deleteStatus == false)
        {
            return NotFound(); 
        } 
        return NoContent();
    }


    [HttpPut]
    [Authorize(AuthenticationSchemes = "Access", Roles = "admin")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO request)
    {
        var updatedCategory= await _categoryService.EditCategory(request);
        return Ok(updatedCategory);
    }
}