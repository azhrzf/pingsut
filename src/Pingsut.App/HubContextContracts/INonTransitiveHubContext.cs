using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.App.HubContextContracts;

public interface INonTransitiveHubContext
{
    Task HubCreateRoom(string creatorUserName, string roomId);
    Task HubJoinRoom(Player player, NonTransitiveRoom room);
    Task HubLeaveRoom(string playerUserName, string roomId);
    Task HubSendMove(string playerUserName, string roomId);
    Task HubLockResult(string playerUserName, string roomId);
    Task HubReceiveResult(string roomId, NonTransitiveResult result);
}