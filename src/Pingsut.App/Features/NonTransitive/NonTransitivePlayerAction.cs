using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public class NonTransitivePlayerAction
{
    public required BasePlayer Player { get; init; }
    public required int ActionId { get; init; }
    public bool LockAction { get; set; } = false;
}