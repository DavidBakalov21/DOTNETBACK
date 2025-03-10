using DotNetAdditional.Contexts;
using DotNetAdditional.DTO.Category;
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

    public async Task<bool> CreatePost(CreatePostDTO post, string email)
    {
        var userId= await getUserIdByEmail(email);
        try
        {
            var newPost = new Post
            {

                UserId = userId,
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
    

    public async Task<ReturnPostDTO[]> GetPosts(int page, int[] categories, bool liked, string email,int? creatorId)
    {
        var userId= await getUserIdByEmail(email);
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
        return await query.Select(p => new ReturnOwnPostDTO
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Content = p.Content,
                Created = p.Created,
                videoURL = p.videoURL,
                Categories = p.PostCategories
                    .Select(pc => new ReturnCategoryDTO
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToArray()
            })
            .Skip((page - 1) * 10)
            .Take(10)
            .ToArrayAsync();  
    }

    public async Task<bool> DeletePost(int id, string email)
    { 
        var userId= await getUserIdByEmail(email);
        var post= await _dbContext.Post.FirstOrDefaultAsync(p=>p.UserId == userId && p.Id == id);
        if (post is null) return false;
        await _dbContext.Post
            .Where(p => p.UserId == userId && p.Id == id)
            .ExecuteDeleteAsync();
        return true;
    }

    public async Task<ReturnOwnPostDTO?> EditPost(UpdatePostDTO newPost, string email)
    {
        var userId= await getUserIdByEmail(email);
        
        var post = await _dbContext.Post
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category) 
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

        return new ReturnOwnPostDTO
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            Created = post.Created,
            videoURL = post.videoURL,
            Categories = post.PostCategories
                .Select(pc => new ReturnCategoryDTO
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name
                }).ToArray() 
        };
    }

    public async Task<ReturnOwnPostDTO[]> GetOwnPosts(int page, int[] categories, string email)
    {
        var userId = await getUserIdByEmail(email);

        var query = _dbContext.Post
            .Where(p => p.UserId == userId)
            .Include(p => p.PostCategories)
            .ThenInclude(pc => pc.Category)
            .AsQueryable();

        if (categories != null && categories.Any())
        {
            query = query.Where(p => p.PostCategories.Any(pc => categories.Contains(pc.Category.Id)));
        }

        return await query
            .Select(p => new ReturnOwnPostDTO
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Content = p.Content,
                Created = p.Created,
                videoURL = p.videoURL,
                likes = p.Likers.Count() ,
                Categories = p.PostCategories
                    .Select(pc => new ReturnCategoryDTO
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToArray()
            })
            .Skip((page - 1) * 10)
            .Take(10)
            .ToArrayAsync();
    }

    public async Task<bool> ToggleLike(int postId, string email)
    {
        var userId= await getUserIdByEmail(email);

        var existingLike = await _dbContext.Likers.FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == userId);

        if (existingLike != null)
        {
            _dbContext.Likers.Remove(existingLike);
            await _dbContext.SaveChangesAsync();
            return false;
        }
        
            var newLike = new LikerTable
            {
                PostId = postId,
                UserId = userId
            };
            _dbContext.Likers.Add(newLike);
            await _dbContext.SaveChangesAsync();
            return true; 
        
    }

    private async Task<int> getUserIdByEmail(string email)
    {
        var user = await _dbContext.User.FirstAsync(p => p.Email == email);
        return user.Id;
    }
}