using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetAdditional.Entities;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public string videoURL { get; set; }
    
    public virtual ICollection<PostCategoryTable> PostCategories { get; set; } = new List<PostCategoryTable>();
    public virtual ICollection<LikerTable> Likers { get; set; } = new List<LikerTable>();
}