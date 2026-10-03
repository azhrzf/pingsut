using JetBrains.Annotations;

namespace Pingsut.App.DTO;

[PublicAPI]
public sealed record LoginRequestDto(
    string UserName,
    string Password );

[PublicAPI]
public sealed record RegisterRequestDto(
    string UserName,
    string Password );