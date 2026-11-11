using GameMatchmaking.Extensions;
using GameMatchmaking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameMatchmaking.Controllers;

[ApiController]
[Route("api/playerStats")]
public class PlayerStatsController : ControllerBase
{
    private readonly GameMatchmaking.Services.StatsService _statsService;

    public PlayerStatsController(GameMatchmaking.Services.StatsService playerService)
    {
        _statsService = playerService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StatsEntry>> GetEntry(int id = 1)
    {
        StatsEntry? entry = await _statsService.GetStatsAsync(id);
        if(entry == null) return NotFound("No player with that ID");

        return Ok(entry);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<StatsEntry>> RegisterPlayer(RegisterPlayerStatsRequest request)
    {
        int id = User.GetPlayerID();
        if(id <= 0) return BadRequest("Player ID can't be 0 or negative value");

        var entry = await _statsService.RegisterPlayerStats(request.matchesPlayed, request.kills, request.deaths, request.assists);
        return Ok(entry);
    }
}