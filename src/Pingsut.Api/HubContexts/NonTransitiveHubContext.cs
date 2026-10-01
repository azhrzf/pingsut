using Microsoft.AspNetCore.SignalR;
using Pingsut.Api.Hubs;
using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;
using Pingsut.App.HubContexts;

namespace Pingsut.Api.HubContexts;

public class NonTransitiveHubContext : INonTransitiveHubContext
{
    private readonly IHubContext<NonTransitiveHub> _hub;

    public NonTransitiveHubContext(
        IHubContext<NonTransitiveHub> hub)
    {
        _hub = hub;
    }

    public async Task HubCreateRoom(string creatorUserName, string roomId)
    {
        await _hub.Groups.AddToGroupAsync(creatorUserName, roomId);
    }

    public async Task HubJoinRoom(Player player, NonTransitiveRoom room)
    {
        await _hub.Groups.AddToGroupAsync(player.UserName, room.Id);
        await _hub.Clients.Group(room.Id).SendAsync("RoomUpdated", new
        {
            RoomId = room.Id,
            room.Players,
            Command = room.NonTransitiveCommand
        });
    }

    public async Task HubLeaveRoom(string playerUserName, string roomId)
    {
        await _hub.Groups.RemoveFromGroupAsync(playerUserName, roomId);
        await _hub.Clients.Group(roomId).SendAsync("PlayerLeft", playerUserName);
    }

    public async Task HubSendMove(string playerUserName, string roomId)
    {
        await _hub.Clients.Group(roomId).SendAsync("PlayerMoved", playerUserName);
    }

    public async Task HubLockResult(string playerUserName, string roomId)
    {
        await _hub.Clients.Group(roomId).SendAsync("PlayerLocked", playerUserName);
    }

    public async Task HubReceiveResult(string roomId, NonTransitiveResult result)
    {
        await _hub.Clients.Group(roomId).SendAsync("ReceiveResult", result);
    }
}