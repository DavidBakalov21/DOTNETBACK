namespace DotNetAdditional.DTO.Post;

public class CreatePostDTO
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string videoURL { get; set; }
    public int UserId { get; set; }
    public int[] CategoryIds { get; set; }
}
