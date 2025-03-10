using DotNetAdditional.DTO.Category;
using DotNetAdditional.DTO.Post;
using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public interface IPostService
{
    public Task<bool> CreatePost(CreatePostDTO post,string email);
    public Task<ReturnPostDTO[]> GetPosts(int page, int[] categories, bool liked, string email,int? creatorId);
    public Task<bool> DeletePost(int id, string email);
    public Task<ReturnOwnPostDTO?> EditPost(UpdatePostDTO newPost, string email);
    public Task<ReturnOwnPostDTO[]> GetOwnPosts(int page,int[] categories,string email);
    
    public Task<bool> ToggleLike(int postId, string email);
}