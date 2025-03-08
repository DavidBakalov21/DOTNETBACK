namespace DotNetAdditional.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<PostCategoryTable> PostCategories { get; set; } = new List<PostCategoryTable>();
}