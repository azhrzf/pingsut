using FluentValidation;

namespace Pingsut.App.Features.NonTransitive.Validators;

public sealed class NonTransitiveCommandValidator : AbstractValidator<NonTransitiveCommand>
{
    public NonTransitiveCommandValidator()
    {
        RuleFor(x => x.PlayerActions)
            .Cascade(CascadeMode.Stop)
            .MustComplyWithPlayerActionValidator();

        RuleFor(x => x.Actions)
            .Cascade(CascadeMode.Stop)
            .Custom(NonTransitiveActionValidatorCustomRule.ValidateDuplicateActions)
            .MustComplyWithActionValidator();

        RuleFor(x => x.Rules)
            .Custom(NonTransitiveRuleValidatorCustomRule.ValidateRules);

        RuleFor(x => x)
            .Custom(NonTransitiveCommandValidationExtensions.ValidateMatchActionsAndRules)
            .Custom(NonTransitiveCommandValidationExtensions.ValidateDefeatsActionsRuleIntegrity)
            .Custom(NonTransitiveCommandValidationExtensions.ValidatePlayerActionsInActions);
    }
}

internal static class NonTransitiveCommandValidationExtensions
{
    internal static void ValidateMatchActionsAndRules(
        NonTransitiveCommand command,
        ValidationContext<NonTransitiveCommand> context)
    {
        var actionIds = command.Actions.Select(a => a.Id).ToHashSet();

        foreach (var rule in command.Rules)
        {
            if (!actionIds.Contains(rule.ActionId))
            {
                context.AddFailure($"Rule ActionId {rule.ActionId} does not exist in actions.");
                return;
            }

            foreach (var defeatActionId in rule.DefeatsActionIds)
            {
                if (!actionIds.Contains(defeatActionId))
                {
                    context.AddFailure(
                        $"DefeatsActionId {defeatActionId} in rule {rule.ActionId} does not exist in actions.");
                    return;
                }
            }
        }

        var ruleActionIds = command.Rules.Select(rule => rule.ActionId).ToHashSet();

        foreach (var action in command.Actions)
        {
            if (!ruleActionIds.Contains(action.Id))
            {
                context.AddFailure($"Found action not found in rules, id: {action.Id}.");
                return;
            }
        }
    }

    internal static void ValidatePlayerActionsInActions(
        NonTransitiveCommand command,
        ValidationContext<NonTransitiveCommand> context)
    {
        var actionIds = command.Actions.Select(a => a.Id).ToHashSet();

        foreach (var playerAction in command.PlayerActions)
        {
            if (!playerAction.LockAction)
            {
                context.AddFailure("PlayerActions must be locked");
                return;
            }

            if (!actionIds.Contains(playerAction.ActionId))
            {
                context.AddFailure($"Player action ActionId {playerAction.ActionId} does not exist in actions.");
                return;
            }
        }
    }

    internal static void ValidateDefeatsActionsRuleIntegrity(
        NonTransitiveCommand command,
        ValidationContext<NonTransitiveCommand> context)
    {
        var validDefeatsPerRule = (command.Actions.Count - 1) / 2;

        foreach (var rule in command.Rules)
        {
            var uniqueDefeatsActions = new HashSet<int>();

            if (rule.DefeatsActionIds.Count != validDefeatsPerRule)
            {
                context.AddFailure($"The number of defeats per rule in {rule.ActionId} is not valid." +
                                   $"Expected {validDefeatsPerRule}, got {rule.DefeatsActionIds.Count}.");

                return;
            }

            foreach (var defeatAction in rule.DefeatsActionIds)
            {
                if (defeatAction == rule.ActionId)
                {
                    context.AddFailure($"Logical error: {rule.ActionId} cannot defeat itself.");
                    return;
                }

                if (!uniqueDefeatsActions.Add(defeatAction))
                {
                    context.AddFailure(
                        $"Found duplicate defeat action id for {rule.ActionId}, id: {defeatAction}.");
                    return;
                }

                var matchingDefeatRule = command.Rules.FirstOrDefault(r => r.ActionId == defeatAction);

                if (matchingDefeatRule is null)
                {
                    context.AddFailure($"Found defeat action not found in rules id: {defeatAction}.");
                    return;
                }

                if (matchingDefeatRule.WinAgainst(rule.ActionId))
                {
                    context.AddFailure(
                        $"Logical error: {matchingDefeatRule.ActionId} is already defeated against {rule.ActionId}.");
                    return;
                }
            }
        }
    }
}