namespace GameMatchmaking.Models;

public record PlayerConnection(int playerID, string connectionID);

public class MatchmakingRoom
{
    public int ID { get; set; }
    public int size { get; set; } = 4;
    public int elo { get; set; }
    public HashSet<int> PlayerIDs = new();

    public bool AddPlayer(int playerID)
    {
        return !IsFull() && PlayerIDs.Add(playerID);        
    }

    public bool IsFull()
    {
        return PlayerIDs.Count >= size;
    }
}