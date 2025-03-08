namespace DotNetAdditional.Entities;

public class LikerTable
{
        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
}