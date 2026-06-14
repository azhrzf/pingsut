using Microsoft.AspNetCore.SignalR;
using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.Api.Hubs;

public class NonTransitiveHub : Hub
{
    private readonly NonTransitiveRoomManager _roomManager;
    private readonly NonTransitiveService _service;

    public NonTransitiveHub(NonTransitiveRoomManager roomManager, NonTransitiveService nonTransitiveService)
    {
        _roomManager = roomManager;
        _service = nonTransitiveService;
    }

    public async Task CreateRoom(NonTransitiveCommand command)
    {
        var newId = Guid.NewGuid().ToString();

        BasePlayer player = new() { Id = Context.ConnectionId };

        await Groups.AddToGroupAsync(Context.ConnectionId, newId);

        _roomManager.CreateRoom(newId, player, command);
    }

    public async Task JoinRoom(string roomId)
    {
        try
        {
            BasePlayer player = new() { Id = Context.ConnectionId };

            _roomManager.JoinRoom(roomId, player);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
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
    }

    public void SendMove(int actionId)
    {
        try
        {
            _roomManager.SendMove(Context.ConnectionId, actionId);
        }
        catch (Exception ex)
        {
            throw new HubException(ex.Message);
        }
    }

    public async Task LockResult(BasePlayer player)
    {
        int lockedCount = _roomManager.LockResult(Context.ConnectionId);

        if (lockedCount == 2)
        {
            var command = _roomManager.GetRoomByPlayerId(Context.ConnectionId).NonTransitiveCommand;

            // return _service.GetResult(command, player);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _roomManager.LeaveRoom(Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}