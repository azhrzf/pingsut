using System.Security.Authentication;
using MapsterMapper;
using Pingsut.App.Domain;
using Pingsut.App.DTO;
using Pingsut.App.Features.NonTransitive;
using Pingsut.App.Features.NonTransitive.Validators.BaseValidators;
using Pingsut.App.Features.NonTransitive.Validators.FeatureValidators;
using Pingsut.App.HubContextContracts;

namespace Pingsut.App.Services;

public interface INonTransitiveService
{
  Task<string> CreateRoom( string? playerUserName, NonTransitiveCreateRoomRequestDto request,
      CancellationToken ct = default );

  Task<NonTransitiveJoinRoomResponseDto> JoinRoom( string roomId, string? playerUserName,
      CancellationToken ct = default );

  Task LeaveRoom( string roomId, string? playerUserName, CancellationToken ct = default );
  Task SendMove( NonTransitiveSendMoveRequestDto request, CancellationToken ct = default );
  Task LockResult( NonTransitiveLockResultRequestDto request, CancellationToken ct = default );
}

public class NonTransitiveService : INonTransitiveService
{
  private readonly INonTransitiveHubContext _hubContext;
  private readonly INonTransitiveRoomStore _roomStore;
  private readonly IMapper _mapper;

  public NonTransitiveService(
      INonTransitiveHubContext hubContext,
      INonTransitiveRoomStore roomStore,
      IMapper mapper )
  {
    _hubContext = hubContext;
    _roomStore = roomStore;
    _mapper = mapper;
  }

  public async Task<string> CreateRoom( string? playerUserName, NonTransitiveCreateRoomRequestDto request,
      CancellationToken ct = default )
  {
    if( playerUserName is null ) throw new AuthenticationException( "Invalid player username" );
    NonTransitiveCreateRoomValidator.Validate( request );

    Player player = new() { UserName = playerUserName };
    string roomId = Guid.NewGuid().ToString();

    await _hubContext.HubCreateRoom( playerUserName, roomId, ct );
    NonTransitiveCommand command = _mapper.Map<NonTransitiveCommand>( request );
    _roomStore.CreateRoom( roomId, player, command );

    return roomId;
  }

  public async Task<NonTransitiveJoinRoomResponseDto> JoinRoom( string roomId, string? playerUserName,
      CancellationToken ct = default )
  {
    bool isAnonymous = playerUserName is null;
    playerUserName ??= Guid.NewGuid().ToString();
    Player player = new() { UserName = playerUserName, IsAnonymous = isAnonymous };

    NonTransitiveRoom room = _roomStore.GetRoomByRoomId( roomId );

    _roomStore.JoinRoom( roomId, player );
    await _hubContext.HubJoinRoom( player, room, ct );

    NonTransitiveRoom updatedRoom = _roomStore.GetRoomByRoomId( roomId );

    return _mapper.Map<NonTransitiveJoinRoomResponseDto>( updatedRoom );
  }

  public async Task LeaveRoom( string roomId, string? playerUserName, CancellationToken ct = default )
  {
    if( playerUserName is null )
    {
      NonTransitiveRoom existingRoom = _roomStore.GetRoomByRoomId( roomId );
      playerUserName = existingRoom.Players.Find( player => player.IsAnonymous )?.UserName;

      if( playerUserName is null ) throw new InvalidOperationException( "Invalid room id or username" );
    }

    _roomStore.LeaveRoom( playerUserName );
    await _hubContext.HubLeaveRoom( playerUserName, roomId, ct );
  }

  public async Task SendMove( NonTransitiveSendMoveRequestDto request, CancellationToken ct = default )
  {
    _roomStore.SendMove( request.PlayerUserName, request.ActionId );
    NonTransitiveRoom room = _roomStore.GetRoomByPlayerUserName( request.PlayerUserName );
    await _hubContext.HubSendMove( request.PlayerUserName, room.Id, ct );
  }

  public async Task LockResult( NonTransitiveLockResultRequestDto request, CancellationToken ct = default )
  {
    int lockedCount = _roomStore.LockResult( request.Player.UserName );
    NonTransitiveRoom room = _roomStore.GetRoomByPlayerUserName( request.Player.UserName );
    await _hubContext.HubLockResult( request.Player.UserName, room.Id, ct );

    if( lockedCount == 2 )
    {
      NonTransitiveCommand command = room.NonTransitiveCommand;
      PenetratedRoomValidator( command, room.Id, ct );

      NonTransitiveResult result = GetResult( command, request.Player );
      await _hubContext.HubReceiveResult( room.Id, result, ct );
      _roomStore.ClearMoves( room.Id );
    }
  }

  private NonTransitiveResult GetResult( NonTransitiveCommand command, Player player )
  {
    NonTransitiveCommandValidator.Validate( command );
    NonTransitivePlayerAction playerAction = command.PlayerActions.Single( p => p.Player.UserName == player.UserName );
    NonTransitivePlayerAction opponentAction =
        command.PlayerActions.Single( p => p.Player.UserName != player.UserName );
    NonTransitiveMatchResult matchPlayerResult = NonTransitiveMatchResult.Draw;
    NonTransitiveMatchResult matchOpponentResult = NonTransitiveMatchResult.Draw;

    if( playerAction.ActionId != opponentAction.ActionId )
    {
      matchPlayerResult = command.Rules.Single( rule => rule.ActionId == playerAction.ActionId )
          .WinAgainst( opponentAction.ActionId )
          ? NonTransitiveMatchResult.Win
          : NonTransitiveMatchResult.Lose;

      matchOpponentResult = matchPlayerResult == NonTransitiveMatchResult.Win
          ? NonTransitiveMatchResult.Lose
          : NonTransitiveMatchResult.Win;
    }

    NonTransitiveAction translatedAction = command.Actions.Single( action => action.Id == playerAction.ActionId );
    NonTransitiveAction translatedOpponentAction =
        command.Actions.Single( action => action.Id == opponentAction.ActionId );

    NonTransitivePlayerResult playerResult = new( playerAction.Player, matchPlayerResult, translatedAction );
    NonTransitivePlayerResult opponentResult =
        new( opponentAction.Player, matchOpponentResult, translatedOpponentAction );

    return new NonTransitiveResult( [playerResult, opponentResult] );
  }

  private void PenetratedRoomValidator( NonTransitiveCommand command, string roomId,
      CancellationToken ct = default )
  {
    try
    {
      NonTransitiveResultValidator.Validate( command );
    }
    catch( NonTransitiveException )
    {
      command.PlayerActions.ForEach( async void ( playerAction ) =>
      {
        await _hubContext.HubRemovePlayersFromRoom( playerAction.Player.UserName, roomId, ct );
      } );

      throw;
    }
  }
}