using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Pingsut.App.Domain;
using Pingsut.App.DTO;

namespace Pingsut.App.Services;

public interface IAuthService
{
    Task CreateAsync(RegisterRequestDto registerRequestDto);
    Task LoginAsync(LoginRequestDto loginRequestDto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<Player> _userManager;
    private readonly SignInManager<Player> _signInManager;
    private readonly IMapper _mapper;

    public AuthService(UserManager<Player> userManager, SignInManager<Player> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task CreateAsync(RegisterRequestDto registerRequestDto)
    {
        var basePlayer = _mapper.Map<Player>(registerRequestDto);
        await _userManager.CreateAsync(basePlayer, registerRequestDto.Password);
    }

    public async Task LoginAsync(LoginRequestDto loginRequestDto)
    {
        var user = await _userManager.FindByNameAsync(loginRequestDto.UserName);
        if (user is null) throw new UnauthorizedAccessException("Invalid username or password");
        await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, true, true);
    }
}