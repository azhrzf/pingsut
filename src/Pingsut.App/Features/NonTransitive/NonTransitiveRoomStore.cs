using FluentValidation;
using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public interface INonTransitiveRoomStore
{
    void CreateRoom(string roomId, Player playerCreator, NonTransitiveCommand command);
    void JoinRoom(string roomId, Player player);
    void LeaveRoom(string playerUserName);
    void SendMove(string playerUserName, int actionId);
    int LockResult(string playerUserName);
    NonTransitiveRoom GetRoomByRoomId(string roomId);
    NonTransitiveRoom GetRoomByPlayerUserName(string playerUserName);
    void ClearMoves(string roomId);
}

public sealed class NonTransitiveRoomStore : INonTransitiveRoomStore
{
    private readonly Dictionary<string, NonTransitiveRoom> _rooms = new();
    private readonly Dictionary<string, string> _connectionRooms = new();

    public void CreateRoom(string roomId, Player playerCreator, NonTransitiveCommand command)
    {
        List<Player> players = [playerCreator];
        NonTransitiveRoom room = new() { Id = roomId, Players = players, NonTransitiveCommand = command };

        _rooms[roomId] = room;
        _connectionRooms[playerCreator.UserName] = roomId;
    }

    public void JoinRoom(string roomId, Player player)
    {
        if (!_rooms.TryGetValue(roomId, out var room))
        {
            throw new ValidationException("Room not found.");
        }

        if (room.IsFull)
        {
            throw new ValidationException("Room is full.");
        }

        if (_connectionRooms.TryGetValue(player.UserName, out _))
        {
            LeaveRoom(player.UserName);
        }

        if (room.Players.Contains(player)) return;
        room.Players.Add(player);
        _connectionRooms[player.UserName] = roomId;
    }

    public void LeaveRoom(string playerUserName)
    {
        if (!_connectionRooms.Remove(playerUserName, out var roomId)) return;
        if (!_rooms.TryGetValue(roomId, out var room)) return;
        room.Players.RemoveAll(p => p.UserName == playerUserName);

        if (room.Players.Count == 0)
        {
            _rooms.Remove(roomId);
        }
    }

    public void SendMove(string playerUserName, int actionId)
    {
        var room = GetRoomByPlayerUserName(playerUserName);
        var player = room.Players.Single(p => p.UserName == playerUserName);
        NonTransitivePlayerAction playerAction = new() { Player = player, ActionId = actionId };
        room.NonTransitiveCommand.PlayerActions.Add(playerAction);
    }

    public int LockResult(string playerUserName)
    {
        var room = GetRoomByPlayerUserName(playerUserName);
        var player = room.Players.Single(p => p.UserName == playerUserName);
        var playerAction = room.NonTransitiveCommand.PlayerActions.Single(p => p.Player.UserName == player.UserName);
        playerAction.LockAction = true;
        return room.NonTransitiveCommand.PlayerActions.Count(p => p.LockAction);
    }

    public NonTransitiveRoom GetRoomByRoomId(string roomId)
    {
        return !_rooms.TryGetValue(roomId, out var room) ? throw new ValidationException("Room not found.") : room;
    }

    public NonTransitiveRoom GetRoomByPlayerUserName(string playerUserName)
    {
        if (_connectionRooms.TryGetValue(playerUserName, out var roomId))
        {
            return _rooms.TryGetValue(roomId, out var room) ? room : throw new ValidationException("Room not found.");
        }

        throw new ValidationException("Player not found.");
    }

    public void ClearMoves(string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) throw new ValidationException("Room not found.");
        room.NonTransitiveCommand.PlayerActions.Clear();
    }
}

public class NonTransitiveRoom
{
    public required string Id { get; init; }
    public required List<Player> Players { get; init; } = [];
    public required NonTransitiveCommand NonTransitiveCommand { get; init; }
    public bool IsFull => Players.Count >= 2;
}