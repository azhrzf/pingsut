using FluentValidation;

namespace Pingsut.App.Features.Game.Mode.NonTransitive;

public class NonTransitiveCommandValidator : AbstractValidator<NonTransitiveCommand>
{
    public NonTransitiveCommandValidator()
    {
        RuleFor(x => x.PlayerActions)
            .Cascade(CascadeMode.Stop)
            .Must(actions => actions.Count == 2)
            .WithMessage("Exactly 2 player are required");

        RuleFor(x => x.Actions)
            .Cascade(CascadeMode.Stop)
            .Must(actions => actions.Count >= 3)
            .WithMessage("At least 3 actions are required")
            .Must(actions => actions.Count % 2 == 1)
            .WithMessage("The number of actions is not balanced. It must be odd.")
            .Custom(ValidateDuplicateActions);

        RuleFor(x => x.Rules)
            .Custom(ValidateRules);

        RuleFor(x => x)
            .Custom(ValidateMatchActionsAndRules)
            .Custom(ValidateDefeatsActionsRuleIntegrity)
            .Custom(ValidatePlayerActionsInActions);
    }

    private void ValidateRules(List<NonTransitiveRule> rules,
        ValidationContext<NonTransitiveCommand> context)
    {
        var uniqueRuleIds = new HashSet<int>();
        var uniqueRuleNames = new HashSet<string>();

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

            // if (!uniqueRuleNames.Add(rule.Action.CommandName))
            // {
            //     context.AddFailure($"Found duplicate rule name for {rule.Action.CommandName}, id: {rule.Action.Id}.");
            //
            //     return;
            // }
        }
    }

    private void ValidateDuplicateActions(List<NonTransitiveAction> actions,
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

    private void ValidateMatchActionsAndRules(NonTransitiveCommand command,
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
                    context.AddFailure($"DefeatsActionId {defeatActionId} in rule {rule.ActionId} does not exist in actions.");

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

    private void ValidatePlayerActionsInActions(NonTransitiveCommand command,
        ValidationContext<NonTransitiveCommand> context)
    {
        var actionIds = command.Actions.Select(a => a.Id).ToHashSet();

        foreach (var playerAction in command.PlayerActions)
        {
            if (!actionIds.Contains(playerAction.ActionId))
            {
                context.AddFailure($"Player action ActionId {playerAction.ActionId} does not exist in actions.");

                return;
            }
        }
    }

    private void ValidateDefeatsActionsRuleIntegrity(NonTransitiveCommand command,
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
                    context.AddFailure(
                        $"Found defeat action not found in rules id: {defeatAction}.");

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