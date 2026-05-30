using FluentValidation;
using MediatR;

namespace Pingsut.App.Features.Game.Mode.NonTransitive;

public class NonTransitiveCommandHandler: IRequestHandler<NonTransitiveCommand, NonTransitiveResult>
{
    private readonly IValidator<NonTransitiveCommand> _validator;
    
    public NonTransitiveCommandHandler(IValidator<NonTransitiveCommand> validator)
    {
        _validator = validator;
    }

    public async Task<NonTransitiveResult> Handle(NonTransitiveCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var playerAction = command.PlayerActions[0];
        var opponentAction = command.PlayerActions[1];

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
