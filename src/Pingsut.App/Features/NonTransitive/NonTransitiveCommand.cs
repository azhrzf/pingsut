using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public sealed class NonTransitiveCommand
{
  public required List<NonTransitiveAction> Actions { get; init; }
  public required List<NonTransitiveRule> Rules { get; init; }
  public List<NonTransitivePlayerAction> PlayerActions { get; init; } = [];
}

public sealed record NonTransitiveAction(
    int Id,
    NonTransitiveActionBaseData Data );

public sealed record NonTransitiveActionBaseData(
    string Name );

public sealed class NonTransitivePlayerAction
{
  public required Player Player { get; init; }
  public required int ActionId { get; set; }
  public bool LockAction { get; set; }
}