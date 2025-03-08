namespace DotNetAdditional.Entities;

public class PostCategoryTable
{
    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }
}