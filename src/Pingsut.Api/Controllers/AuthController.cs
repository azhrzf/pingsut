using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pingsut.App.DTO;
using Pingsut.App.Services;

namespace Pingsut.Api.Controllers;

[ApiController]
[ApiVersion( "1.0" )]
[Route( "api/v{version:apiVersion}/[controller]" )]
[Produces( "application/json" )]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController( IAuthService authService )
  {
    _authService = authService;
  }

  [HttpPost( "register" )]
  [AllowAnonymous]
  [ProducesResponseType( StatusCodes.Status200OK )]
  [ProducesResponseType( StatusCodes.Status400BadRequest )]
  public async Task<IActionResult> Register( [FromBody] RegisterRequestDto registerRequestDto )
  {
    await _authService.CreateAsync( registerRequestDto );

    return Created();
  }

  [HttpPost( "login" )]
  [AllowAnonymous]
  [ProducesResponseType( StatusCodes.Status200OK )]
  [ProducesResponseType( StatusCodes.Status401Unauthorized )]
  public async Task<IActionResult> Login( [FromBody] LoginRequestDto loginRequestDto )
  {
    await _authService.LoginAsync( loginRequestDto );

    return Ok();
  }
}