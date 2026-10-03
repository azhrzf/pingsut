using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pingsut.App.DTO;
using Pingsut.App.Services;

namespace Pingsut.Api.Controllers;

[ApiController]
[ApiVersion( "1.0" )]
[Route( "api/v{version:apiVersion}/non-transitive" )]
[Produces( "application/json" )]
public class NonTransitiveController : ControllerBase
{
  private readonly INonTransitiveService _nonTransitiveService;

  public NonTransitiveController( INonTransitiveService nonTransitiveService )
  {
    _nonTransitiveService = nonTransitiveService;
  }

  [HttpPost( "room/create" )]
  [Authorize]
  [Consumes( "application/json" )]
  [Produces( "application/json" )]
  [ProducesResponseType( StatusCodes.Status200OK )]
  [ProducesResponseType( StatusCodes.Status400BadRequest )]
  [ProducesResponseType( StatusCodes.Status404NotFound )]
  [ProducesResponseType( StatusCodes.Status422UnprocessableEntity )]
  public async Task<IActionResult> CreateRoom(
      NonTransitiveCreateRoomRequestDto request,
      CancellationToken cancellationToken = default )
  {
    string? userName = User.FindFirstValue( ClaimTypes.Name );
    string response = await _nonTransitiveService.CreateRoom( userName, request, cancellationToken );
    return Ok( response );
  }

  [HttpPost( "room/{roomId}/join" )]
  [AllowAnonymous]
  [Consumes( "application/json" )]
  [Produces( "application/json" )]
  [ProducesResponseType( StatusCodes.Status200OK )]
  [ProducesResponseType( StatusCodes.Status400BadRequest )]
  [ProducesResponseType( StatusCodes.Status404NotFound )]
  [ProducesResponseType( StatusCodes.Status422UnprocessableEntity )]
  public async Task<IActionResult> JoinRoom(
      string roomId,
      CancellationToken cancellationToken = default )
  {
    string? userName = User.FindFirstValue( ClaimTypes.Name );
    NonTransitiveJoinRoomResponseDto response =
        await _nonTransitiveService.JoinRoom( roomId, userName, cancellationToken );
    return Ok( response );
  }

  [HttpPost( "room/{roomId}/leave" )]
  [AllowAnonymous]
  [Consumes( "application/json" )]
  [Produces( "application/json" )]
  [ProducesResponseType( StatusCodes.Status200OK )]
  [ProducesResponseType( StatusCodes.Status400BadRequest )]
  [ProducesResponseType( StatusCodes.Status404NotFound )]
  [ProducesResponseType( StatusCodes.Status422UnprocessableEntity )]
  public async Task<IActionResult> LeaveRoom(
      string roomId,
      CancellationToken cancellationToken = default )
  {
    string? userName = User.FindFirstValue( ClaimTypes.Name );
    await _nonTransitiveService.LeaveRoom( roomId, userName, cancellationToken );
    return Ok();
  }
}