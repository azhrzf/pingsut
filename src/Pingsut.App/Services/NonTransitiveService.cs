using FluentValidation;
using MapsterMapper;
using Pingsut.App.Domain;
using Pingsut.App.DTO;
using Pingsut.App.Features.NonTransitive;
using Pingsut.App.HubContextContracts;

namespace Pingsut.App.Services;

public interface INonTransitiveService
{
    Task<string> CreateRoom(NonTransitiveCreateRoomRequestDto request);
    Task JoinRoom(NonTransitiveJoinRoomRequestDto request);
    Task LeaveRoom(NonTransitiveLeaveRoomRequestDto request);
    Task SendMove(NonTransitiveSendMoveRequestDto request);
    Task LockResult(NonTransitiveLockResultRequestDto request);
}

public class NonTransitiveService : INonTransitiveService
{
    private readonly INonTransitiveHubContext _hubContext;
    private readonly INonTransitiveRoomStore _roomStore;
    private readonly IValidator<NonTransitiveCommand> _validator;
    private readonly IMapper _mapper;

    public NonTransitiveService(
        INonTransitiveHubContext hubContext,
        INonTransitiveRoomStore roomStore,
        IValidator<NonTransitiveCommand> validator,
        IMapper mapper)
    {
        _hubContext = hubContext;
        _roomStore = roomStore;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<string> CreateRoom(NonTransitiveCreateRoomRequestDto request)
    {
        var roomId = Guid.NewGuid().ToString();
        await _hubContext.HubCreateRoom(request.PlayerCreator.UserName, roomId);
        var command = _mapper.Map<NonTransitiveCommand>(request);
        _roomStore.CreateRoom(roomId, request.PlayerCreator, command);
        return roomId;
    }

    public async Task JoinRoom(NonTransitiveJoinRoomRequestDto request)
    {
        var room = _roomStore.GetRoomByRoomId(request.RoomId);
        _roomStore.JoinRoom(request.RoomId, request.Player);
        await _hubContext.HubJoinRoom(request.Player, room);
    }

    public async Task LeaveRoom(NonTransitiveLeaveRoomRequestDto request)
    {
        _roomStore.LeaveRoom(request.PlayerUserName);
        await _hubContext.HubLeaveRoom(request.PlayerUserName, request.RoomId);
    }

    public async Task SendMove(NonTransitiveSendMoveRequestDto request)
    {
        _roomStore.SendMove(request.PlayerUserName, request.ActionId);
        var room = _roomStore.GetRoomByPlayerUserName(request.PlayerUserName);
        await _hubContext.HubSendMove(request.PlayerUserName, room.Id);
    }

    public async Task LockResult(NonTransitiveLockResultRequestDto request)
    {
        var lockedCount = _roomStore.LockResult(request.Player.UserName);
        var room = _roomStore.GetRoomByPlayerUserName(request.Player.UserName);
        await _hubContext.HubLockResult(request.Player.UserName, room.Id);

        if (lockedCount == 2)
        {
            var command = room.NonTransitiveCommand;
            var result = await GetResult(command, request.Player);
            await _hubContext.HubReceiveResult(room.Id, result);
            _roomStore.ClearMoves(room.Id);
        }
    }

    private async Task<NonTransitiveResult> GetResult(NonTransitiveCommand command, Player player)
    {
        await _validator.ValidateAndThrowAsync(command);
        var playerAction = command.PlayerActions.Single(p => p.Player.UserName == player.UserName);
        var opponentAction = command.PlayerActions.Single(p => p.Player.UserName != player.UserName);
        var matchPlayerResult = NonTransitiveMatchResult.Draw;
        var matchOpponentResult = NonTransitiveMatchResult.Draw;

        if (playerAction.ActionId != opponentAction.ActionId)
        {
            matchPlayerResult = command.Rules.Single(rule => rule.ActionId == playerAction.ActionId)
                .WinAgainst(opponentAction.ActionId)
                ? NonTransitiveMatchResult.Win
                : NonTransitiveMatchResult.Lose;

            matchOpponentResult = matchPlayerResult == NonTransitiveMatchResult.Win
                ? NonTransitiveMatchResult.Lose
                : NonTransitiveMatchResult.Win;
        }

        var translatedAction = command.Actions.Single(action => action.Id == playerAction.ActionId);
        var translatedOpponentAction = command.Actions.Single(action => action.Id == opponentAction.ActionId);

        NonTransitivePlayerResult playerResult = new(playerAction.Player, matchPlayerResult, translatedAction);
        NonTransitivePlayerResult opponentResult =
            new(opponentAction.Player, matchOpponentResult, translatedOpponentAction);

        return new NonTransitiveResult([playerResult, opponentResult]);
    }
}