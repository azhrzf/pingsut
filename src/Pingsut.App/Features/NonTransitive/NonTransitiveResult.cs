using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public sealed record NonTransitiveResult(
    NonTransitivePlayerResult[] PlayerResults);

public sealed record NonTransitivePlayerResult(
    Player Player,
    NonTransitiveMatchResult Result,
    NonTransitiveAction Action);

public enum NonTransitiveMatchResult
{
    Win,
    Lose,
    Draw
}