namespace DotNetAdditional.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    
    public string Role { get; set; }
    public string Password { get; set; }
    public virtual ICollection<LikerTable> LikedPosts { get; set; } = new List<LikerTable>();
    
}