using Microsoft.AspNetCore.Identity;

namespace Pingsut.App.Domain;

public class BasePlayer : IdentityUser;

public class PlayerWithConnectionId : BasePlayer
{
    public required string ConnectionId { get; init; }
}