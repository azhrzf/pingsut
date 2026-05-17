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

        var result = playerAction.Action.Id == opponentAction.Action.Id
            ? NonTransitiveEnumResult.Draw
            : command.Rules.Single(rule => rule.Action.Id == playerAction.Action.Id)
                .WinAgainst(opponentAction.Action.Id)
                ? NonTransitiveEnumResult.Win
                : NonTransitiveEnumResult.Lose;

        return new NonTransitiveResult
        {
            Player = playerAction.Player,
            Action = playerAction.Action,
            Result = result
        };
    }
}
