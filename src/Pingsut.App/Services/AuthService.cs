using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Pingsut.App.DTO;

namespace Pingsut.App.Services;

public interface IAuthService
{
  Task CreateAsync( RegisterRequestDto registerRequestDto );
  Task LoginAsync( LoginRequestDto loginRequestDto );
}

public class AuthService : IAuthService
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly SignInManager<IdentityUser> _signInManager;
  private readonly IMapper _mapper;

  public AuthService(
      UserManager<IdentityUser> userManager,
      SignInManager<IdentityUser> signInManager,
      IMapper mapper )
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _mapper = mapper;
  }

  public async Task CreateAsync( RegisterRequestDto registerRequestDto )
  {
    IdentityUser user = _mapper.Map<IdentityUser>( registerRequestDto );
    await _userManager.CreateAsync( user, registerRequestDto.Password );
  }

  public async Task LoginAsync( LoginRequestDto loginRequestDto )
  {
    IdentityUser? user = await _userManager.FindByNameAsync( loginRequestDto.UserName );
    if( user is null ) throw new UnauthorizedAccessException( "Invalid username or password" );
    await _signInManager.PasswordSignInAsync( user, loginRequestDto.Password, true, true );
  }
}