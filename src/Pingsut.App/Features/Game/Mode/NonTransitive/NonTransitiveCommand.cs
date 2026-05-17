using MediatR;

namespace Pingsut.App.Features.Game.Mode.NonTransitive;

public class NonTransitiveCommand : IRequest<NonTransitiveResult>
{
    public required List<NonTransitiveAction> Actions { get; init; }
    public required List<NonTransitiveRule> Rules { get; init; }
    public List<NonTransitivePlayerAction> PlayerActions { get; init; }

    public NonTransitiveCommand(List<NonTransitivePlayerAction> playerActions)
    {
        if (playerActions.Count != 2)
        {
            throw new InvalidOperationException("PlayerActions must have 2 elements");
        }

        PlayerActions = playerActions;
    }
}
