using DotNetAdditional.DTO.Category;
using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public interface ICategoryService
{
    public Task<bool> CreateCategory(CreateCategoryDTO category);
    public Task<ReturnCategoryDTO[]> GetCategory();
    public Task<bool> DeleteCategory(int id);
    public Task<ReturnCategoryDTO?> EditCategory(UpdateCategoryDTO newCategory);
}