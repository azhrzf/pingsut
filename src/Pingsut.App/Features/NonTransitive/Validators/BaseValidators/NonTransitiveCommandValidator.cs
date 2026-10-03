namespace Pingsut.App.Features.NonTransitive.Validators.BaseValidators;

public static class NonTransitiveCommandValidator
{
  private const string ERROR_TITLE = "command";

  public static void Validate( NonTransitiveCommand command )
  {
    List<string> errors =
    [
        .. NonTransitivePlayerActionValidator.ValidatePlayerActionsCount( command.PlayerActions ),
        .. NonTransitiveActionValidator.ValidateActionsCount( command.Actions ),
        .. NonTransitiveActionValidator.ValidateDuplicateActions( command.Actions ),
        .. NonTransitiveRuleValidator.ValidateRulesIntegrity( command.Rules, command.Actions ),
        .. ValidateMatchActionsAndRules( command ),
        .. ValidateDefeatsActionsRuleIntegrity( command ),
        .. ValidatePlayerActionsInActions( command )
    ];

    Dictionary<string, string[]> result = [];

    if( errors.Count == 0 ) return;
    result.Add( ERROR_TITLE, [.. errors] );

    if( result.Count > 0 ) throw new NonTransitiveException( result );
  }

  public static List<string> ValidateMatchActionsAndRules( NonTransitiveCommand command )
  {
    List<string> errors = [];

    HashSet<int> actionIds = [.. command.Actions.Select( a => a.Id )];

    foreach( NonTransitiveRule rule in command.Rules )
    {
      string actionName = errors.GetActionName( command.Actions, rule.ActionId );

      if( !actionIds.Contains( rule.ActionId ) )
      {
        errors.Add( $"Rule action id: {rule.ActionId}, name: {actionName} does not exist in actions." );
      }

      foreach( int defeatActionId in rule.DefeatsActionIds )
      {
        if( !actionIds.Contains( defeatActionId ) )
        {
          string defeatActionName = errors.GetActionName( command.Actions, rule.ActionId );
          errors.Add(
              $"Defeats action id: {defeatActionId}, name: {defeatActionName} in " +
              $"rule id: {rule.ActionId}, name: {actionName} does not exist in actions." );
        }
      }
    }

    HashSet<int> ruleActionIds = [.. command.Rules.Select( rule => rule.ActionId )];

    foreach( NonTransitiveAction action in command.Actions )
    {
      if( !ruleActionIds.Contains( action.Id ) )
      {
        errors.Add( $"Found action not found in rules, id: {action.Id}, name: {action.Data.Name}." );
      }
    }

    return errors;
  }

  public static List<string> ValidateDefeatsActionsRuleIntegrity( NonTransitiveCommand command )
  {
    List<string> errors = [];

    int validDefeatsPerRule = ( command.Actions.Count - 1 ) / 2;

    foreach( NonTransitiveRule rule in command.Rules )
    {
      HashSet<int> uniqueDefeatsActions = [];

      if( rule.DefeatsActionIds.Count != validDefeatsPerRule )
      {
        string actionName = errors.GetActionName( command.Actions, rule.ActionId );
        errors.Add( $"The number of defeats per rule in id: {rule.ActionId}, name: {actionName} is not valid. " +
                    $"Expected {validDefeatsPerRule}, got {rule.DefeatsActionIds.Count}." );
      }

      foreach( int defeatAction in rule.DefeatsActionIds )
      {
        string actionName = errors.GetActionName( command.Actions, defeatAction );

        if( defeatAction == rule.ActionId )
        {
          errors.Add( $"Logical error: action id: {rule.ActionId}, name: {actionName} cannot defeat itself." );
        }

        if( !uniqueDefeatsActions.Add( defeatAction ) )
        {
          errors.Add(
              $"Found duplicate defeat action id for {rule.ActionId}, id: {defeatAction}, name: {actionName}." );
        }

        NonTransitiveRule? matchingDefeatRule = command.Rules.FirstOrDefault( r => r.ActionId == defeatAction );

        if( matchingDefeatRule is null )
        {
          errors.Add( $"Defeat action not found in rules id: {defeatAction}, name: {actionName}." );
        }

        if( matchingDefeatRule != null && matchingDefeatRule.WinAgainst( rule.ActionId ) )
        {
          string properWinName = errors.GetActionName( command.Actions, rule.ActionId );
          errors.Add( $"Logical error: action id: {matchingDefeatRule.ActionId}," +
                      $"name: {actionName} is already defeated against id: {rule.ActionId}, name: {properWinName}." );
        }
      }
    }

    return errors;
  }

  public static List<string> ValidatePlayerActionsInActions( NonTransitiveCommand command )
  {
    List<string> errors = [];

    HashSet<int> actionIds = command.Actions.Select( a => a.Id ).ToHashSet();

    foreach( NonTransitivePlayerAction playerAction in command.PlayerActions )
    {
      if( !playerAction.LockAction )
      {
        errors.Add( $"Player {playerAction.Player.UserName} action must be locked" );
      }

      if( !actionIds.Contains( playerAction.ActionId ) )
      {
        string actionName = errors.GetActionName( command.Actions, playerAction.ActionId );
        errors.Add( $"Player action id: {playerAction.ActionId}, name: {actionName} does not exist in actions." );
      }
    }

    return errors;
  }
}