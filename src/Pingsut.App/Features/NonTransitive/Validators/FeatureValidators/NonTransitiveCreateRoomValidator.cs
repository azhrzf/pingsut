using Pingsut.App.DTO;
using Pingsut.App.Features.NonTransitive.Validators.BaseValidators;

namespace Pingsut.App.Features.NonTransitive.Validators.FeatureValidators;

public static class NonTransitiveCreateRoomValidator
{
  private const string ERROR_TITLE = "createRoom";

  public static void Validate( NonTransitiveCreateRoomRequestDto request )
  {
    List<string> errors =
    [
        .. NonTransitiveActionValidator.ValidateActionsCount( request.Actions ),
        .. NonTransitiveActionValidator.ValidateDuplicateActions( request.Actions ),
        .. NonTransitiveRuleValidator.ValidateRulesIntegrity( request.Rules, request.Actions )
    ];

    Dictionary<string, string[]> result = [];

    if( errors.Count == 0 ) return;
    result.Add( ERROR_TITLE, [.. errors] );

    if( result.Count > 0 ) throw new NonTransitiveException( result );
  }
}