using JetBrains.Annotations;

namespace Pingsut.App.DTO;

[PublicAPI]
public record LoginRequestDto(string Email, string Password);
