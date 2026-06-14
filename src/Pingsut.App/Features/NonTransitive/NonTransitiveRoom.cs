using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveRoom
{
    public required string Id { get; init; }
    public required List<BasePlayer> Players { get; init; } = new();
    public required NonTransitiveCommand NonTransitiveCommand { get; init; } 

    public bool IsFull => Players.Count >= 2;
}