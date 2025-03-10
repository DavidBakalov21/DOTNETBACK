using DotNetAdditional.DTO.Category;

namespace DotNetAdditional.DTO.Post;

public class ReturnPostDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public string videoURL { get; set; }
   
    public ReturnCategoryDTO[] Categories { get; set; }
   
}