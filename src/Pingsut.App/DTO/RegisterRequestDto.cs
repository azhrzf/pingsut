using JetBrains.Annotations;

namespace Pingsut.App.DTO;

[PublicAPI]
public record RegisterRequestDto(string UserName, string Email, string Password);