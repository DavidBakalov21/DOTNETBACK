namespace DotNetAdditional.DTO.Post;

public class UpdatePostDTO
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string videoURL { get; set; }
    public int[] CategoryIds { get; set; }
}