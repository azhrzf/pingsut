using Pingsut.Domain.Common;
using Pingsut.Domain.Entities.Common;

namespace Pingsut.Domain.Entities;

public class User : AuditableEntity
{
    public required string FirebaseId { get; set; }
}