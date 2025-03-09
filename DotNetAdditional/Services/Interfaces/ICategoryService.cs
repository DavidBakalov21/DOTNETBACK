using DotNetAdditional.DTO.Category;
using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public interface ICategoryService
{
    public Task<bool> CreateCategory(CreateCategoryDTO category);
    public Task<Category[]> GetCategory(int page);
    public Task<bool> DeleteCategory(int id);
    public Task<Category?> EditCategory(UpdateCategoryDTO newCategory);
}