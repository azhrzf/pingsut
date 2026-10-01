using Microsoft.AspNetCore.SignalR;

namespace Pingsut.Api.Hubs;

public class NonTransitiveHub : Hub;

    // public override async Task OnDisconnectedAsync(Exception? exception)
    // {
    //     try
    //     {
    //         var room = _roomManager.GetRoomByPlayerId(Context.ConnectionId);
    //         if (room != null)
    //         {
    //             _roomManager.LeaveRoom(Context.ConnectionId);
    //             await Clients.Group(room.Id).SendAsync("PlayerLeft", Context.ConnectionId);
    //             await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Id);
    //         }
    //     }
    //     catch
    //     {
    //         // Ignore if player/room not found during disconnect cleanup
    //     }
    //
    //     await base.OnDisconnectedAsync(exception);
    // }