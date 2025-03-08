using DotNetAdditional.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetAdditional.Contexts;


public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> User { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Post> Post { get; set; }
    public DbSet<PostCategoryTable> PostCategories { get; set; }
    public DbSet<LikerTable> Likers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostCategoryTable>()
            .HasKey(pc => new { pc.PostId, pc.CategoryId }); 

        modelBuilder.Entity<PostCategoryTable>()
            .HasOne(pc => pc.Post)
            .WithMany(p => p.PostCategories)
            .HasForeignKey(pc => pc.PostId);

        modelBuilder.Entity<PostCategoryTable>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.PostCategories) 
            .HasForeignKey(pc => pc.CategoryId);
        
        modelBuilder.Entity<LikerTable>()
            .HasKey(pl => new { pl.PostId, pl.UserId });

        modelBuilder.Entity<LikerTable>()
            .HasOne(pl => pl.Post)
            .WithMany(p => p.Likers)
            .HasForeignKey(pl => pl.PostId);

        modelBuilder.Entity<LikerTable>()
            .HasOne(pl => pl.User)
            .WithMany(u => u.LikedPosts)
            .HasForeignKey(pl => pl.UserId);
    }
}