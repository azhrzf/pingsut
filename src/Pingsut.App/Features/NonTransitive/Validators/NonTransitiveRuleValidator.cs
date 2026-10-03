using FluentValidation;

namespace Pingsut.App.Features.NonTransitive.Validators;

public sealed class NonTransitiveRuleValidator : AbstractValidator<NonTransitiveCommand>
{
    public NonTransitiveRuleValidator()
    {
        RuleFor(x => x.Rules)
            .Cascade(CascadeMode.Stop)
            .Custom(NonTransitiveRuleValidatorCustomRule.ValidateRules);
    }
}

internal static class NonTransitiveRuleValidatorCustomRule
{
    internal static void ValidateRules(
        List<NonTransitiveRule> rules,
        ValidationContext<NonTransitiveCommand> context)
    {
        var uniqueRuleIds = new HashSet<int>();

        foreach (var rule in rules)
        {
            if (rule.DefeatsActionIds.Count == 0)
            {
                context.AddFailure("DefeatsActions cannot be empty");
                return;
            }

            if (!uniqueRuleIds.Add(rule.ActionId))
            {
                context.AddFailure($"Found duplicate rule id for id: {rule.ActionId}.");
                return;
            }
        }
    }
}