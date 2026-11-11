using GameMatchmaking.Services;
using GameMatchmaking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameMatchmaking.Extensions;
using System.Reflection.Metadata.Ecma335;

namespace GameMatchmaking.Hubs;

[Authorize]
public class MatchmakingHub : Hub
{
    private readonly MatchmakingService _service;

    public MatchmakingHub(MatchmakingService service)
    {
        _service = service;
    }

    public override async Task OnConnectedAsync()
    {
        if(Context.User != null) await _service.TryRegisterPlayer(new PlayerConnection(Context.User.GetPlayerID(), Context.ConnectionId));
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if(Context.User != null) _service.RemovePlayer(Context.User.GetPlayerID());
        return base.OnDisconnectedAsync(exception);
    }
}