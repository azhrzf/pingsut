using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Pingsut.App.Domain;
using Pingsut.App.DTO;

namespace Pingsut.App.Services;

public interface IAuthService
{
    Task CreateAsync(RegisterRequestDto registerRequestDto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<BasePlayer> _userManager;
    private readonly IMapper _mapper;

    public AuthService(UserManager<BasePlayer> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task CreateAsync(RegisterRequestDto registerRequestDto)
    {
        var basePlayer = _mapper.Map<BasePlayer>(registerRequestDto);

        await _userManager.CreateAsync(basePlayer, registerRequestDto.Password);
    }
}