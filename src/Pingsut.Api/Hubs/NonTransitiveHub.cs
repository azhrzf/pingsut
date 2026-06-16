using Microsoft.AspNetCore.SignalR;
using Pingsut.App.Contracts;
using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.Api.Hubs;

public class NonTransitiveHub : Hub
{
    private readonly NonTransitiveRoomManager _roomManager;
    private readonly INonTransitiveService _service;

    public NonTransitiveHub(NonTransitiveRoomManager roomManager, INonTransitiveService nonTransitiveService)
    {
        _roomManager = roomManager;
        _service = nonTransitiveService;
    }

    public async Task<string> CreateRoom(NonTransitiveCommand command)
    {
        var newId = Guid.NewGuid().ToString();

        BasePlayer player = new() { Id = Context.ConnectionId };

        await Groups.AddToGroupAsync(Context.ConnectionId, newId);

        _roomManager.CreateRoom(newId, player, command);

        return newId;
    }

    public async Task JoinRoom(string roomId)
    {
        try
        {
            BasePlayer player = new() { Id = Context.ConnectionId };

            _roomManager.JoinRoom(roomId, player);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            var room = _roomManager.GetRoomByPlayerId(Context.ConnectionId);
            
            await Clients.Group(roomId).SendAsync("RoomUpdated", new
            {
                RoomId = room.Id,
                Players = room.Players,
                Command = room.NonTransitiveCommand
            });
        }
        catch (Exception ex)
        {
            throw new HubException(ex.Message);
        }
    }

    public async Task LeaveRoom(string roomId)
    {
        _roomManager.LeaveRoom(Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        await Clients.Group(roomId).SendAsync("PlayerLeft", Context.ConnectionId);
    }

    public async Task SendMove(int actionId)
    {
        try
        {
            _roomManager.SendMove(Context.ConnectionId, actionId);
            
            var room = _roomManager.GetRoomByPlayerId(Context.ConnectionId);
            
            await Clients.Group(room.Id).SendAsync("PlayerMoved", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            throw new HubException(ex.Message);
        }
    }

    public async Task LockResult(BasePlayer player)
    {
        var lockedCount = _roomManager.LockResult(Context.ConnectionId);
        var room = _roomManager.GetRoomByPlayerId(Context.ConnectionId);

        await Clients.Group(room.Id).SendAsync("PlayerLocked", Context.ConnectionId);

        if (lockedCount == 2)
        {
            var command = room.NonTransitiveCommand;

            var result = await _service.GetResult(command, player);
            
            await Clients.Group(room.Id).SendAsync("ReceiveResult", result);
            
            _roomManager.ClearMoves(room.Id);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var room = _roomManager.GetRoomByPlayerId(Context.ConnectionId);
            if (room != null)
            {
                _roomManager.LeaveRoom(Context.ConnectionId);
                await Clients.Group(room.Id).SendAsync("PlayerLeft", Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Id);
            }
        }
        catch
        {
            // Ignore if player/room not found during disconnect cleanup
        }

        await base.OnDisconnectedAsync(exception);
    }
}