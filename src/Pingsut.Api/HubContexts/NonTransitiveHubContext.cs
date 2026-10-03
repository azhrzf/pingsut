using Microsoft.AspNetCore.SignalR;
using Pingsut.Api.Hubs;
using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;
using Pingsut.App.HubContextContracts;

namespace Pingsut.Api.HubContexts;

public class NonTransitiveHubContext : INonTransitiveHubContext
{
  private readonly IHubContext<NonTransitiveHub> _hub;

  public NonTransitiveHubContext( IHubContext<NonTransitiveHub> hub )
  {
    _hub = hub;
  }

  public async Task HubCreateRoom( string creatorUserName, string roomId, CancellationToken ct = default )
  {
    await _hub.Groups.AddToGroupAsync( creatorUserName, roomId, ct );
  }

  public async Task HubJoinRoom( Player player, NonTransitiveRoom room, CancellationToken ct = default )
  {
    await _hub.Groups.AddToGroupAsync( player.UserName, room.Id, ct );
    await _hub.Clients.Group( room.Id ).SendAsync( "RoomUpdated", new
    {
      RoomId = room.Id,
      room.Players,
      Command = room.NonTransitiveCommand
    }, cancellationToken: ct );
  }

  public async Task HubLeaveRoom( string playerUserName, string roomId, CancellationToken ct = default )
  {
    await _hub.Groups.RemoveFromGroupAsync( playerUserName, roomId, ct );
    await _hub.Clients.Group( roomId ).SendAsync( "PlayerLeft", playerUserName, cancellationToken: ct );
  }

  public async Task HubSendMove( string playerUserName, string roomId, CancellationToken ct = default )
  {
    await _hub.Clients.Group( roomId ).SendAsync( "PlayerMoved", playerUserName, cancellationToken: ct );
  }

  public async Task HubLockResult( string playerUserName, string roomId, CancellationToken ct = default )
  {
    await _hub.Clients.Group( roomId ).SendAsync( "PlayerLocked", playerUserName, cancellationToken: ct );
  }

  public async Task HubReceiveResult( string roomId, NonTransitiveResult result, CancellationToken ct = default )
  {
    await _hub.Clients.Group( roomId ).SendAsync( "ReceiveResult", result, cancellationToken: ct );
  }

  public async Task HubRemovePlayersFromRoom( string playerUserName, string roomId,
      CancellationToken ct = default )
  {
    await _hub.Groups.RemoveFromGroupAsync( playerUserName, roomId, ct );
  }
}