using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pingsut.App.Features.Game.Mode.NonTransitive;

namespace Pingsut.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class NonTransitiveController : ControllerBase
{
    private readonly ISender _sender;

    public NonTransitiveController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("result")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetGameResult(NonTransitiveCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }
}