using Pingsut.App.Features.Game.Base.Players;

namespace Pingsut.App.Features.Game.Mode.NonTransitive;

public class NonTransitiveResult
{
    public required BasePlayer Player { get; set; }
    public required NonTransitiveAction Action { get; set; }
    public required string Result { get; set; }
}