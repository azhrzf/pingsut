namespace Pingsut.App.Domain;

public sealed class Player
{
  public required string UserName { get; init; }
  public bool IsAnonymous { get; init; }
}