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

        var matchPlayerResult = NonTransitiveEnumResult.Draw;
        var matchOpponentResult = NonTransitiveEnumResult.Draw;

        if (playerAction.ActionId != opponentAction.ActionId)
        {
            matchPlayerResult = command.Rules.Single(rule => rule.ActionId == playerAction.ActionId)
                .WinAgainst(opponentAction.ActionId)
                ? NonTransitiveEnumResult.Win
                : NonTransitiveEnumResult.Lose;

            matchOpponentResult = matchPlayerResult == NonTransitiveEnumResult.Win
                ? NonTransitiveEnumResult.Lose
                : NonTransitiveEnumResult.Win;
        }

        var translatedAction = command.Actions.Single(action => action.Id == playerAction.ActionId);
        var translatedOpponentAction = command.Actions.Single(action => action.Id == opponentAction.ActionId);

        NonTransitivePlayerResult playerResult = new()
        {
            Player = playerAction.Player,
            Action = translatedAction,
            Result = matchPlayerResult
        };

        NonTransitivePlayerResult opponentResult = new()
        {
            Player = opponentAction.Player,
            Action = translatedOpponentAction,
            Result = matchOpponentResult
        };


        return new NonTransitiveResult
        {
            Players = [playerResult, opponentResult]
        };
    }
}