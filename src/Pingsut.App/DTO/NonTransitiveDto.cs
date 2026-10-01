using JetBrains.Annotations;
using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.App.DTO;

[PublicAPI]
public sealed record NonTransitiveCreateRoomRequestDto(
    Player PlayerCreator,
    List<NonTransitiveAction> Actions,
    List<NonTransitiveRule> Rules);

[PublicAPI]
public sealed record NonTransitiveJoinRoomRequestDto(
    Player Player,
    string RoomId);

[PublicAPI]
public sealed record NonTransitiveLeaveRoomRequestDto(
    string PlayerUserName,
    string RoomId);

[PublicAPI]
public sealed record NonTransitiveSendMoveRequestDto(
    string PlayerUserName,
    string RoomId,
    int ActionId);

[PublicAPI]
public sealed record NonTransitiveLockResultRequestDto(
    Player Player);