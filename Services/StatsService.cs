using GameMatchmaking.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<StatsEntry> RegisterPlayerStats(int matchesPlayed, int Kills, int Deaths, int Assists)
    {
        var entry = new StatsEntry
        {
            MatchesPlayed = matchesPlayed,
            Kills = Kills,
            Deaths = Deaths,
            Assists = Assists,
            Elo = 0
        };

        _Database.stats.Add(entry);
        await _Database.SaveChangesAsync();
        return entry;
    }
}