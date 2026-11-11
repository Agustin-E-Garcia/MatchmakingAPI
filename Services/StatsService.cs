using GameMatchmaking.Models;

namespace GameMatchmaking.Services;

public class StatsService
{
    private readonly GameMatchmaking.Data.StatsDbContext _Database;

    public StatsService(GameMatchmaking.Data.StatsDbContext Database)
    {
        _Database = Database;
    }

    public ValueTask<StatsEntry?> GetStatsAsync(int id)
    {
        return _Database.stats.FindAsync(id);
    }

    public async Task<StatsEntry> InitializePlayerStats(int playerID)
    {
        var entry = new StatsEntry
        {
            Id = playerID,
            MatchesPlayed = 0,
            Kills = 0,
            Deaths = 0,
            Assists = 0,
            Elo = 0
        };
        _Database.stats.Add(entry);
        await _Database.SaveChangesAsync();
        return entry;
    }

    public async Task<StatsEntry?> RegisterPlayerStats(int id, int Kills, int Deaths, int Assists)
    {
        StatsEntry? stats = await _Database.stats.FindAsync(id);
        if(stats is null) return stats;

        stats.MatchesPlayed++;
        stats.Kills += Kills;
        stats.Deaths += Deaths;
        stats.Assists += Assists;
        stats.Elo = (stats.Kills + stats.Assists) / int.Max(stats.Deaths, 1);

        _Database.stats.Update(stats);
        await _Database.SaveChangesAsync();
        return stats;
    }
}