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
            .Custom(ValidateDefeatsActionsRuleIntegrity);
    }

    private void ValidateRules(List<NonTransitiveRule> rules,
        ValidationContext<NonTransitiveCommand> context)
    {
        var uniqueRuleIds = new HashSet<int>();
        var uniqueRuleNames = new HashSet<string>();

        foreach (var rule in rules)
        {
            if (!uniqueRuleIds.Add(rule.Action.Id))
            {
                context.AddFailure($"Found duplicate rule id for {rule.Action.CommandName}, id: {rule.Action.Id}.");

                return;
            }

            if (!uniqueRuleNames.Add(rule.Action.CommandName))
            {
                context.AddFailure($"Found duplicate rule name for {rule.Action.CommandName}, id: {rule.Action.Id}.");

                return;
            }
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
                context.AddFailure($"Found duplicate action id for {action.CommandName}, id: {action.Id}.");

                return;
            }

            if (!uniqueActionNames.Add(action.CommandName))
            {
                context.AddFailure($"Found duplicate action name for {action.CommandName}, id: {action.Id}.");

                return;
            }
        }
    }

    private void ValidateMatchActionsAndRules(NonTransitiveCommand command,
        ValidationContext<NonTransitiveCommand> context)
    {
        var seen = new HashSet<(int Id, string Name)>();

        command.Rules.ForEach(rule => seen.Add((rule.Action.Id, rule.Action.CommandName)));

        foreach (var action in command.Actions.Where(action => !seen.Contains((action.Id, action.CommandName))))
        {
            context.AddFailure($"Found action not found in rules: {action.CommandName}, id: {action.Id}.");

            return;
        }
    }

    private void ValidateDefeatsActionsRuleIntegrity(NonTransitiveCommand command,
        ValidationContext<NonTransitiveCommand> context)
    {
        var validDefeatsPerRule = (command.Actions.Count - 1) / 2;

        foreach (var rule in command.Rules)
        {
            var uniqueDefeatsActions = new HashSet<int>();

            if (rule.DefeatsActions.Count != validDefeatsPerRule)
            {
                context.AddFailure($"The number of defeats per rule in {rule.Action.CommandName} is not valid." +
                                   $"Expected {validDefeatsPerRule}, got {rule.DefeatsActions.Count}.");

                return;
            }

            foreach (var defeatAction in rule.DefeatsActions)
            {
                if (defeatAction.Id == rule.Action.Id)
                {
                    context.AddFailure($"Logical error: {rule.Action.CommandName} cannot defeat itself.");

                    return;
                }

                if (!uniqueDefeatsActions.Add(defeatAction.Id))
                {
                    context.AddFailure(
                        $"Found duplicate defeat action id for {rule.Action.CommandName}, id: {defeatAction.Id}.");

                    return;
                }

                var matchingDefeatRule = command.Rules.FirstOrDefault(r => r.Action.Id == defeatAction.Id);

                if (matchingDefeatRule is null)
                {
                    context.AddFailure(
                        $"Found defeat action not found in rules: {defeatAction.CommandName}, id: {defeatAction.Id}.");

                    return;
                }

                if (matchingDefeatRule.WinAgainst(rule.Action.Id))
                {
                    context.AddFailure(
                        $"Logical error: {matchingDefeatRule.Action.CommandName} is already defeated against {rule.Action.CommandName}.");

                    return;
                }
            }
        }
    }
}