using Microsoft.EntityFrameworkCore;
using Domain;
namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<ConfigurationEntity> Configurations { get; set; }
    public DbSet<SaveGameEntity> SaveGames { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationEntity>().ToTable("Configurations");
        modelBuilder.Entity<SaveGameEntity>().ToTable("SaveGames");
    }
}