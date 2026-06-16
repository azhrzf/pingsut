using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveResult
{
    public required NonTransitivePlayerResult[] Players { get; init; }
}

public class NonTransitivePlayerResult
{
    public required BasePlayer Player { get; set; }
    public required NonTransitiveEnumResult Result { get; set; }
    public required NonTransitiveAction Action { get; set; }
    
}