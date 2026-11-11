using Microsoft.AspNetCore.SignalR;
using GameMatchmaking.Hubs;
using GameMatchmaking.Models;
using System.Runtime.CompilerServices;

namespace GameMatchmaking.Services;

public class MatchmakingService
{
    private readonly IHubContext<MatchmakingHub> _hubContext;
    private readonly object _dictionaryLock = new();
    private readonly object _roomLock = new();
    private Dictionary<int, string> _playerConnections = new();
    private List<MatchmakingRoom> _activeRooms = new();

    public MatchmakingService(IHubContext<MatchmakingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task FindMatch(int playerID)
    {
        MatchmakingRoom? playerRoom = null;
        lock(_roomLock)
        {
            foreach(MatchmakingRoom Room in _activeRooms)
            {
                if(!Room.AddPlayer(playerID)) continue;
                playerRoom = Room;
                break;
            }   
        }

        if(playerRoom is null)
        {
            playerRoom = new MatchmakingRoom();
            playerRoom.ID = _activeRooms.Count;
            playerRoom.AddPlayer(playerID);
            lock(_roomLock)
            {
                _activeRooms.Add(playerRoom);   
            }
        }
        else if(playerRoom.IsFull())
        {
            List<string> connections = new();
            foreach(int ID in playerRoom.PlayerIDs)
            {
                connections.Add(_playerConnections[ID]);
            }

            lock(_roomLock)
            {
                _activeRooms.Remove(playerRoom);
            }

            await NotifyMatchFound(connections, $"Room-{playerRoom.ID}");
        }
    }

    public async Task NotifyMatchFound(List<string> connectionIDs, string roomName)
    {
        await _hubContext.Clients.Clients(connectionIDs).SendAsync("MatchFound", roomName);
    }

    public async Task<bool> TryRegisterPlayer(PlayerConnection connection)
    {
        lock(_dictionaryLock)
        {
            if(!_playerConnections.TryAdd(connection.playerID, connection.connectionID)) return false;
        }

        await FindMatch(connection.playerID);
        return true;
    }

    public void RemovePlayer(int playerID)
    {
        lock(_dictionaryLock)
        {
            _playerConnections.Remove(playerID);
        }
    }
}