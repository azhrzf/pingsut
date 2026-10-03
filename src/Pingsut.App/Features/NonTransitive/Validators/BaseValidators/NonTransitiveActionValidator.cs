namespace Pingsut.App.Features.NonTransitive.Validators.BaseValidators;

public static class NonTransitiveActionValidator
{
  private const string ERROR_TITLE = "action";

  public static void Validate( List<NonTransitiveAction> actions )
  {
    List<string> errors =
    [
        .. ValidateActionsCount( actions ),
        .. ValidateDuplicateActions( actions )
    ];

    Dictionary<string, string[]> result = [];

    if( errors.Count == 0 ) return;
    result.Add( ERROR_TITLE, [.. errors] );

    if( result.Count > 0 ) throw new NonTransitiveException( result );
  }

  public static List<string> ValidateActionsCount( List<NonTransitiveAction> actions )
  {
    List<string> errors = [];

    if( actions is not { Count: >= 3 } )
    {
      errors.Add( "At least 3 actions are required." );
    }

    if( actions.Count % 2 != 1 )
    {
      errors.Add( "The number of actions is not balanced. It must be odd." );
    }

    return errors;
  }

  public static List<string> ValidateDuplicateActions( List<NonTransitiveAction> actions )
  {
    List<string> errors = [];

    HashSet<int> uniqueActionIds = [];
    HashSet<string> uniqueActionNames = [];

    foreach( NonTransitiveAction action in actions )
    {
      if( !uniqueActionIds.Add( action.Id ) )
      {
        errors.Add( $"Found duplicate action id for id: {action.Id}." );
      }

      if( !uniqueActionNames.Add( action.Data.Name ) )
      {
        errors.Add( $"Found duplicate action name for {action.Data.Name}, id: {action.Id}." );
      }
    }

    return errors;
  }
}