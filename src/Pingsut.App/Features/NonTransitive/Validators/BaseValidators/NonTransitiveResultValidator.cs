namespace Pingsut.App.Features.NonTransitive.Validators.BaseValidators;

public static class NonTransitiveResultValidator
{
  private const string ERROR_TITLE = "result";

  public static void Validate( NonTransitiveCommand command )
  {
    List<string> errors =
    [
        .. ValidateMaxOnlyOneAnonymousPlayer( command.PlayerActions )
    ];

    Dictionary<string, string[]> result = [];

    if( errors.Count == 0 ) return;
    result.Add( ERROR_TITLE, [.. errors] );

    if( result.Count > 0 ) throw new NonTransitiveException( result );
  }

  #region Potentialy Penetrated System Errors

  public static List<string> ValidateMaxOnlyOneAnonymousPlayer( List<NonTransitivePlayerAction> playerActions )
  {
    List<string> errors = [];

    List<string> anonymousPlayers = ( List<string> )playerActions
        .Where( playerAction => playerAction.Player.IsAnonymous )
        .Select( playerAction => playerAction.Player.UserName );

    if( anonymousPlayers.Count > 1 )
    {
      string invalidPlayersUsername = string.Join( ", ", anonymousPlayers );
      errors.Add( $"INVALID GAME!, THESE PLAYERS {invalidPlayersUsername} NOT SUPPOSED IN THE SAME ROOM" );
    }

    return errors;
  }

  #endregion
}