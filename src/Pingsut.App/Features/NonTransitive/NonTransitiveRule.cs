using JetBrains.Annotations;

namespace Pingsut.App.Features.NonTransitive;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class NonTransitiveRule
{
    public required int ActionId { get; init; }
    public required List<int> DefeatsActionIds { get; init; }

    public bool WinAgainst(int id)
    {
        return DefeatsActionIds.Any(c => c == id);
    }
}