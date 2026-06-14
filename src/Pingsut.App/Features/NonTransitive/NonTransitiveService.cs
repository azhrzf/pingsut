using FluentValidation;
using Pingsut.App.Contracts;
using Pingsut.App.Domain;

namespace Pingsut.App.Features.NonTransitive;

public class NonTransitiveService : INonTransitiveService
{
    private readonly IValidator<NonTransitiveCommand> _validator;

    public NonTransitiveService(IValidator<NonTransitiveCommand> validator)
    {
        _validator = validator;
    }

    public async Task<NonTransitiveResult> GetResult(NonTransitiveCommand command, BasePlayer player)
    {
        await _validator.ValidateAndThrowAsync(command);

        var playerAction = command.PlayerActions.Single(p => p.Player.Id == player.Id);
        var opponentAction = command.PlayerActions.Single(p => p.Player.Id != player.Id);

        var result = playerAction.ActionId == opponentAction.ActionId
            ? NonTransitiveEnumResult.Draw
            : command.Rules.Single(rule => rule.ActionId == playerAction.ActionId)
                .WinAgainst(opponentAction.ActionId)
                ? NonTransitiveEnumResult.Win
                : NonTransitiveEnumResult.Lose;

        var translatedAction = command.Actions.Single(action => action.Id == playerAction.ActionId);

        return new NonTransitiveResult
        {
            Player = playerAction.Player,
            Action = translatedAction,
            Result = result.ToString()
        };
    }
}