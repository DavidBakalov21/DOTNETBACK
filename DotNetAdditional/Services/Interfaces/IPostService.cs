using DotNetAdditional.DTO.Post;
using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public interface IPostService
{
    public Task<bool> CreatePost(CreatePostDTO post);
    public Task<Post[]> GetPosts(int page, int[] categories, bool liked, string email,int? creatorId);
    public Task<bool> DeletePost(int id, string email);
    public Task<Post?> EditPost(UpdatePostDTO newPost, string email);
}