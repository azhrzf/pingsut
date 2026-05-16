using Pingsut.Domain.Common;

namespace Pingsut.Domain.Entities.Common;

public class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}