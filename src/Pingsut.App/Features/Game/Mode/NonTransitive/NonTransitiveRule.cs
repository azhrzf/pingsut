namespace Pingsut.App.Features.Game.Mode.NonTransitive;

public class NonTransitiveRule
{
    public required NonTransitiveAction Action { get; init; }
    public List<NonTransitiveAction> DefeatsActions { get; }

    public NonTransitiveRule(List<NonTransitiveAction> defeatActions)
    {
        if (defeatActions.Count == 0)
        {
            throw new InvalidOperationException("DefeatsActions cannot be empty");
        }

        DefeatsActions = defeatActions;
    }

    public bool WinAgainst(int id)
    {
        return DefeatsActions.Any(c => c.Id == id);
    }
}