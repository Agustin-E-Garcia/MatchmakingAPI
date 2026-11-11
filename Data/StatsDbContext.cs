using GameMatchmaking.Models;
using Microsoft.EntityFrameworkCore;

namespace GameMatchmaking.Data;

public class StatsDbContext : DbContext
{
    public StatsDbContext(DbContextOptions<StatsDbContext> options) : base(options) {}
    public DbSet<StatsEntry> stats => Set<StatsEntry>();
}