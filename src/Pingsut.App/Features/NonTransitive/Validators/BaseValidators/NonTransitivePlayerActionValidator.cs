namespace Pingsut.App.Features.NonTransitive.Validators.BaseValidators;

public static class NonTransitivePlayerActionValidator
{
  private const string ERROR_TITLE = "playerAction";

  public static void Validate( List<NonTransitivePlayerAction> playerActions )
  {
    List<string> errors =
    [
        .. ValidatePlayerActionsCount( playerActions )
    ];

    Dictionary<string, string[]> result = [];

    if( errors.Count == 0 ) return;
    result.Add( ERROR_TITLE, [.. errors] );

    if( result.Count > 0 ) throw new NonTransitiveException( result );
  }

  public static List<string> ValidatePlayerActionsCount( List<NonTransitivePlayerAction> playerActions )
  {
    List<string> errors = [];

    if( playerActions is not { Count: 2 } )
    {
      errors.Add( "Exactly 2 player are required for player actions." );
    }

    return errors;
  }
}