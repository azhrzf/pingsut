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
    private readonly UserManager<BasePlayer> _userManager;
    private readonly SignInManager<BasePlayer> _signInManager;
    private readonly IMapper _mapper;

    public AuthService(UserManager<BasePlayer> userManager, SignInManager<BasePlayer> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task CreateAsync(RegisterRequestDto registerRequestDto)
    {
        var basePlayer = _mapper.Map<BasePlayer>(registerRequestDto);

        await _userManager.CreateAsync(basePlayer, registerRequestDto.Password);
    }

    public async Task LoginAsync(LoginRequestDto loginRequestDto)
    {
        var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);

        if (user is null) throw new UnauthorizedAccessException("Invalid username or password");

        await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, true, true);
    }
}