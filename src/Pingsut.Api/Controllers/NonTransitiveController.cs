using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pingsut.App.Contracts;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class NonTransitiveController : ControllerBase
{
    private readonly INonTransitiveService _gameService;

    public NonTransitiveController(INonTransitiveService gameService)
    {
        _gameService = gameService;
    }

    // [HttpPost("result")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    // public async Task<IActionResult> GetGameResult(NonTransitiveCommand command,
    //     CancellationToken cancellationToken = default)
    // {
    //     var result = await _gameService.PlayGame(command, cancellationToken);
    //
    //     return Ok(result);
    // }
}