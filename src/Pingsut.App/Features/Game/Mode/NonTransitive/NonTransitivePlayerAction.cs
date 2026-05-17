using Pingsut.App.Features.Game.Base.Players;

namespace Pingsut.App.Features.Game.Mode.NonTransitive;

public class NonTransitivePlayerAction
{
    public required BasePlayer Player { get; init; }
    public required NonTransitiveAction Action { get; init; }
}