using JetBrains.Annotations;
using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.App.DTO;

#region Requests

[PublicAPI]
public sealed record NonTransitiveCreateRoomRequestDto(
    List<NonTransitiveAction> Actions,
    List<NonTransitiveRule> Rules );

[PublicAPI]
public sealed record NonTransitiveSendMoveRequestDto(
    string PlayerUserName,
    string RoomId,
    int ActionId );

[PublicAPI]
public sealed record NonTransitiveLockResultRequestDto(
    Player Player );

#endregion // Requests

#region Responses

[PublicAPI]
public sealed record NonTransitiveJoinRoomResponseDto(
    string Id,
    List<Player> Players
);

#endregion // Responses