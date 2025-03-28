using System.Security.Claims;
using DotNetAdditional.DTO.Post;
using DotNetAdditional.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAdditional.Controller;
[ApiController]
[Route("post")]
public class PostController:ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO request)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var status =await _postService.CreatePost(request, userEmail);
        if (status)
        {
            return StatusCode(200);
        }

        return StatusCode(500);
    }


    [HttpGet]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetPosts(int page, string? categories, bool liked,int? creatorId)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        Console.WriteLine(categories);
        var post = await _postService.GetPosts(page, categories, liked,userEmail, creatorId);
        return Ok(post);
    }
    
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = "Access")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            
            var deleteStatus = await _postService.DeletePost(id, userEmail);
            if (deleteStatus == false)
            {
                return NotFound(); 
            } 
            return NoContent();
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = "Access")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostDTO request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var updatedPost= await _postService.EditPost(request, userEmail);
            return Ok(updatedPost);
        }

    [HttpGet("ownPosts")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetOwnPosts(int page, string? categories, bool liked)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var post = await _postService.GetOwnPosts(page, categories, userEmail);
        return Ok(post);
    }
    
    [HttpPost("toggleLike")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> ToggleLike([FromBody] PostIdDTO postId)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var status = await _postService.ToggleLike(postId.Id, userEmail);
        return Ok(status);
    }
}