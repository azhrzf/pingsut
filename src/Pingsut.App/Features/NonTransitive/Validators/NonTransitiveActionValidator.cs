using FluentValidation;

namespace Pingsut.App.Features.NonTransitive.Validators;

public sealed class NonTransitiveActionValidator : AbstractValidator<NonTransitiveCommand>
{
    public NonTransitiveActionValidator()
    {
        RuleFor(x => x.Actions)
            .Cascade(CascadeMode.Stop)
            .Custom(NonTransitiveActionValidatorCustomRule.ValidateDuplicateActions)
            .MustComplyWithActionValidator();
    }
}

internal static class NonTransitiveActionValidationExtensions
{
    internal static void MustComplyWithActionValidator<T>(
        this IRuleBuilder<T, IList<NonTransitiveAction>> ruleBuilder)
    {
        ruleBuilder
            .NotNull()
            .Must(actions => actions.Count >= 3)
            .WithMessage("At least 3 actions are required")
            .Must(actions => actions.Count % 2 == 1)
            .WithMessage("The number of actions is not balanced. It must be odd.");
    }
}

internal static class NonTransitiveActionValidatorCustomRule
{
    internal static void ValidateDuplicateActions(
        List<NonTransitiveAction> actions,
        ValidationContext<NonTransitiveCommand> context)
    {
        var uniqueActionIds = new HashSet<int>();
        var uniqueActionNames = new HashSet<string>();

        foreach (var action in actions)
        {
            if (!uniqueActionIds.Add(action.Id))
            {
                context.AddFailure($"Found duplicate action id for id: {action.Id}.");
                return;
            }

            if (!uniqueActionNames.Add(action.Data.Name))
            {
                context.AddFailure($"Found duplicate action name for {action.Data.Name}, id: {action.Id}.");
                return;
            }
        }
    }
}