namespace Pingsut.App.Features.NonTransitive.Validators.BaseValidators;

public static class NonTransitiveRuleValidator
{
  private const string ERROR_TITLE = "rule";

  public static void Validate( List<NonTransitiveRule> rules, List<NonTransitiveAction> actions )
  {
    List<string> errors =
    [
        .. ValidateRulesIntegrity( rules, actions )
    ];

    Dictionary<string, string[]> result = [];

    if( errors.Count == 0 ) return;
    result.Add( ERROR_TITLE, [.. errors] );

    if( result.Count > 0 ) throw new NonTransitiveException( result );
  }

  public static string GetActionName( this List<string> errors, List<NonTransitiveAction> actions,
      int actionId )
  {
    string? actionName = actions.Find( action => action.Id == actionId )?.Data.Name;

    if( actionName is not null )
    {
      return actionName;
    }

    errors.Add( $"Invalid action name for rule action id: {actionId}." );
    actionName = "Invalid name";

    return actionName;
  }

  public static List<string> ValidateRulesIntegrity( List<NonTransitiveRule> rules, List<NonTransitiveAction> actions )
  {
    List<string> errors = [];
    HashSet<int> uniqueRuleIds = [];

    foreach( NonTransitiveRule rule in rules )
    {
      string actionName = errors.GetActionName( actions, rule.ActionId );

      if( rule.DefeatsActionIds.Count == 0 )
      {
        errors.Add( $"Defeats actions cannot be empty for action: {actionName}, id: {rule.ActionId}." );
      }

      if( !uniqueRuleIds.Add( rule.ActionId ) )
      {
        errors.Add( $"Found duplicate rule id for action: {actionName} id: {rule.ActionId}." );
      }
    }

    return errors;
  }
}