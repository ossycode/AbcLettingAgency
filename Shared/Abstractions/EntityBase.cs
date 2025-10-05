using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.Shared.Abstractions;

public abstract class EntityBase : IEntityBase
{
    [Key]
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? UserDeletedId { get; set; }
    public Guid? UserUpdatedId { get; set; }
}