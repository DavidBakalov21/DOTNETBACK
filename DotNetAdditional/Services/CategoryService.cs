using DotNetAdditional.Contexts;
using DotNetAdditional.DTO.Category;
using DotNetAdditional.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetAdditional.Services;

public class CategoryService:ICategoryService
{
    private readonly DBContext _dbContext;
    public CategoryService(DBContext context)
    {
        _dbContext=context;
    }
    public async Task<bool> CreateCategory(CreateCategoryDTO category)
    {
        try
        {
            var newCategory = new Category
            {
            Name = category.CategoryName
            };
            await _dbContext.Category.AddAsync(newCategory
            );
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }

    }

    public async Task<ReturnCategoryDTO[]> GetCategory()
    {
        return await _dbContext.Category
            .Select(c => new ReturnCategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToArrayAsync(); 
    }

    public async Task<bool> DeleteCategory(int id)
    {
        await _dbContext.Category
            .Where(c =>  c.Id == id)
            .ExecuteDeleteAsync();
        return true;
    }

    public async Task<ReturnCategoryDTO?> EditCategory(UpdateCategoryDTO newCategory)
    {
        var category = await _dbContext.Category
            .Include(p => p.PostCategories) 
            .FirstOrDefaultAsync(p =>  p.Id == newCategory.Id);

        if (category is null) return null;
        
        category.Name = newCategory.newCategoryName;
        await _dbContext.SaveChangesAsync();

        return new ReturnCategoryDTO
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}