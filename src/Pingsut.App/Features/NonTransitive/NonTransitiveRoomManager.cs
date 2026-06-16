using FluentValidation;
using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveRoomManager
{
    private readonly Dictionary<string, NonTransitiveRoom> _rooms = new();
    private readonly Dictionary<string, string> _connectionRooms = new();

    public void CreateRoom(string roomId, BasePlayer playerCreator, NonTransitiveCommand command)
    {
        List<BasePlayer> players = [playerCreator];
        NonTransitiveRoom room = new() { Id = roomId, Players = players, NonTransitiveCommand = command };

        _rooms[roomId] = room;
        _connectionRooms[playerCreator.Id] = roomId;
    }

    public void JoinRoom(string roomId, BasePlayer player)
    {
        if (!_rooms.TryGetValue(roomId, out var room))
        {
            throw new ValidationException("Room not found.");
        }

        if (room.IsFull)
        {
            throw new ValidationException("Room is full.");
        }

        if (_connectionRooms.TryGetValue(player.Id, out var existingRoomId))
        {
            LeaveRoom(player.Id);
        }

        if (room.Players.Contains(player)) return;

        room.Players.Add(player);

        _connectionRooms[player.Id] = roomId;
    }

    public void LeaveRoom(string playerConnectionId)
    {
        if (!_connectionRooms.Remove(playerConnectionId, out var roomId)) return;

        if (!_rooms.TryGetValue(roomId, out var room)) return;

        room.Players.RemoveAll(p => p.Id == playerConnectionId);

        if (room.Players.Count == 0)
        {
            _rooms.Remove(roomId);
        }
    }

    public void SendMove(string playerConnectionId, int actionId)
    {
        var room = GetRoomByPlayerId(playerConnectionId);

        var player = room.Players.Single(p => p.Id == playerConnectionId);

        NonTransitivePlayerAction playerAction = new() { Player = player, ActionId = actionId };

        room.NonTransitiveCommand.PlayerActions.Add(playerAction);
    }

    public int LockResult(string playerConnectionId)
    {
        var room = GetRoomByPlayerId(playerConnectionId);

        var player = room.Players.Single(p => p.Id == playerConnectionId);

        var playerAction = room.NonTransitiveCommand.PlayerActions.Single(p => p.Player.Id == player.Id);
        
        playerAction.LockAction = true;
        
        return room.NonTransitiveCommand.PlayerActions.Count(p => p.LockAction);
    }

    public NonTransitiveRoom GetRoomByPlayerId(string playerConnectionId)
    {
        if (_connectionRooms.TryGetValue(playerConnectionId, out var roomId))
        {
            if (_rooms.TryGetValue(roomId, out var room))
            {
                return room;
            }

            throw new ValidationException("Room not found.");
        }

        throw new ValidationException("Player not found.");
    }
    
    public void ClearMoves(string roomId)
    {
        if (_rooms.TryGetValue(roomId, out var room))
        {
            room.NonTransitiveCommand.PlayerActions.Clear();
            
            return;
        }
        
        throw new ValidationException("Room not found.");
    }
}