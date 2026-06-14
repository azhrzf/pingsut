using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveResult
{
    public required BasePlayer Player { get; set; }
    public required NonTransitiveAction Action { get; set; }
    public required string Result { get; set; }
}