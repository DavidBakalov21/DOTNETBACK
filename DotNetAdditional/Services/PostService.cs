using DotNetAdditional.Contexts;
using DotNetAdditional.DTO.Post;
using DotNetAdditional.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetAdditional.Services;

public class PostService:IPostService
{
    private readonly DBContext _dbContext;
    public PostService(DBContext context)
    {
        _dbContext = context;
    }

    public async Task<bool> CreatePost(CreatePostDTO post)
    {
        try
        {
            var newPost = new Post
            {

                UserId = post.UserId,
                Title = post.Title,
                Content = post.Content,
                Created = DateTime.UtcNow,
                videoURL = post.videoURL
            };
            await _dbContext.Post.AddAsync(newPost
            );
            await _dbContext.SaveChangesAsync();
            if (post.CategoryIds.Length <= 0) return true;
            foreach (var categoryId in post.CategoryIds)
            {
                var postCategory = new PostCategoryTable
                {
                    PostId = newPost.Id,
                    CategoryId = categoryId
                };

                await _dbContext.PostCategories.AddAsync(postCategory);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
       
    }
    

    public async Task<Post[]> GetPosts(int page, int[] categories, bool liked, string email,int? creatorId)
    {
        var user = await _dbContext.User.FirstAsync(p => p.Email == email);
        var userId=user.Id;
        var query = _dbContext.Post
            .Include(p => p.PostCategories)
            .ThenInclude(pc => pc.Category)
            .AsQueryable(); 

      
        if (creatorId.HasValue)
        {
            query = query.Where(p => p.UserId == creatorId.Value);
        }

       
        if (categories != null && categories.Any())
        {
            query = query.Where(p => p.PostCategories.Any(pc => categories.Contains(pc.Category.Id)));
        }

        if (liked)
        {
            query = query.Where(p=>p.Likers.Any(l=>l.UserId == userId) );
        }
        return await query
            .Skip((page - 1) * 10)
            .Take(10)
            .ToArrayAsync();  
    }

    public async Task<bool> DeletePost(int id, string email)
    { 
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return false; 
        var userId=user.Id;
        var post= await _dbContext.Post.FirstOrDefaultAsync(p=>p.UserId == userId && p.Id == id);
        if (post is null) return false;
        await _dbContext.Post
            .Where(p => p.UserId == userId && p.Id == id)
            .ExecuteDeleteAsync();
        return true;
    }

    public async Task<Post?> EditPost(UpdatePostDTO newPost, string email)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return null;
        var userId = user.Id;
        
        var post = await _dbContext.Post
            .Include(p => p.PostCategories) 
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == newPost.PostId);

        if (post is null) return null;
        
        post.Title = newPost.Title;
        post.Content = newPost.Content;
        post.videoURL = newPost.videoURL;
        
        
        _dbContext.PostCategories.RemoveRange(post.PostCategories);

        if (newPost.CategoryIds != null && newPost.CategoryIds.Length > 0)
        {
            foreach (var categoryId in newPost.CategoryIds)
            {
                _dbContext.PostCategories.Add(new PostCategoryTable
                {
                    PostId = post.Id,
                    CategoryId = categoryId
                });
            }
        }
        
        await _dbContext.SaveChangesAsync();

        return post;
    }
}