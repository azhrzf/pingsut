namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveCommand
{
    public required List<NonTransitiveAction> Actions { get; init; }
    public required List<NonTransitiveRule> Rules { get; init; }
    public List<NonTransitivePlayerAction> PlayerActions { get; init; } = [];
}