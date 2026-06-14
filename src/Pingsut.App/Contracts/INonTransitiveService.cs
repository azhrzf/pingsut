using Pingsut.App.Domain;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.App.Contracts;

public interface INonTransitiveService
{
    Task<NonTransitiveResult> GetResult(NonTransitiveCommand command, BasePlayer player);
}