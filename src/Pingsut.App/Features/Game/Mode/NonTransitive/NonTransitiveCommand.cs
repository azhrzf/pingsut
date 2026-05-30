using JetBrains.Annotations;
using MediatR;

namespace Pingsut.App.Features.Game.Mode.NonTransitive;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class NonTransitiveCommand : IRequest<NonTransitiveResult>
{
    public required List<NonTransitiveAction> Actions { get; init; }
    public required List<NonTransitiveRule> Rules { get; init; }
    public required List<NonTransitivePlayerAction> PlayerActions { get; init; }
}