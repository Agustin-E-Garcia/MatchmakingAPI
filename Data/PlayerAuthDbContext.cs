using GameMatchmaking.Models;
using Microsoft.EntityFrameworkCore;

namespace GameMatchmaking.Data;

public class PlayerAuthDbContext : DbContext
{
    public PlayerAuthDbContext(DbContextOptions<PlayerAuthDbContext> options) : base(options) {}
    public DbSet<PlayerEntry> player => Set<PlayerEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerEntry>()
            .HasIndex(p => p.Username)
            .IsUnique();
    }
}