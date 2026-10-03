using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.App.HubContextContracts;

public interface INonTransitiveHubContext
{
  Task HubCreateRoom( string creatorUserName, string roomId, CancellationToken ct = default );
  Task HubJoinRoom( Player player, NonTransitiveRoom room, CancellationToken ct = default );
  Task HubLeaveRoom( string playerUserName, string roomId, CancellationToken ct = default );
  Task HubSendMove( string playerUserName, string roomId, CancellationToken ct = default );
  Task HubLockResult( string playerUserName, string roomId, CancellationToken ct = default );
  Task HubReceiveResult( string roomId, NonTransitiveResult result, CancellationToken ct = default );
}