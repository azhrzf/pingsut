namespace Pingsut.App.Features.NonTransitive;

public sealed class NonTransitiveException : Exception
{
  public Dictionary<string, string[]> Errors { get; }

  public NonTransitiveException( Dictionary<string, string[]> errors )
  {
    Errors = errors;
  }
}