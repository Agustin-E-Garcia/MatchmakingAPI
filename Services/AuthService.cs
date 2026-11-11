using GameMatchmaking.Data;
using GameMatchmaking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameMatchmaking.Services;

public class AuthService
{
    private readonly PlayerAuthDbContext _database;
    private readonly TokenService _tokens;
    private readonly PasswordHasher<PlayerEntry> _hasher = new();
    private readonly StatsService _stats;

    public AuthService(PlayerAuthDbContext database, TokenService tokens, StatsService stats)
    {
        _database = database;
        _tokens = tokens;
        _stats = stats;
    }

    public async Task<AuthResponse?> RegisterAsync(string username, string password)
    {
        if(await _database.player.AnyAsync(p => p.Username == username))
            return null;

        var player = new PlayerEntry{ Username = username, CreatedAt = DateTime.UtcNow };
        player.PasswordHash = _hasher.HashPassword(player, password);

        _database.player.Add(player);
        await _database.SaveChangesAsync();

        await _stats.RegisterPlayerStats(0, 0, 0, 0);

        return new AuthResponse(_tokens.CreateToken(player), player.Username);
    }

    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        var player = await _database.player.FirstOrDefaultAsync(p => p.Username == username);
        if(player is null) return null;

        var result = _hasher.VerifyHashedPassword(player, player.PasswordHash, password);
        if(result == PasswordVerificationResult.Failed)
            return null;

        return new AuthResponse(_tokens.CreateToken(player), player.Username);
    }
}