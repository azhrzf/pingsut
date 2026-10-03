using FluentValidation;

namespace Pingsut.App.Features.NonTransitive.Validators;

public sealed class NonTransitivePlayerActionValidator : AbstractValidator<NonTransitiveCommand>
{
    public NonTransitivePlayerActionValidator()
    {
        RuleFor(x => x.PlayerActions)
            .Cascade(CascadeMode.Stop)
            .MustComplyWithPlayerActionValidator();
    }
}

internal static class NonTransitivePlayerActionValidationExtensions
{
    internal static void MustComplyWithPlayerActionValidator<T>(
        this IRuleBuilder<T, IList<NonTransitivePlayerAction>> ruleBuilder)
    {
        ruleBuilder
            .NotNull()
            .Must(actions => actions is { Count: 2 })
            .WithMessage("Exactly 2 player are required");
    }
}