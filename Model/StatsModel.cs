namespace GameMatchmaking.Models;

public class StatsEntry
{
    public int Id { get; set; }
    public int MatchesPlayed { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public int Elo { get; set; }
}

public record RegisterPlayerStatsRequest(int kills, int deaths, int assists);