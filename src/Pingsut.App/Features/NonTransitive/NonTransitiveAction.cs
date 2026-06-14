namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveAction
{
    public int Id { get; set; }
    public required NonTransitiveActionBaseData Data { get; set; }
}

public class NonTransitiveActionBaseData
{
    public required string Name { get; init; }
}