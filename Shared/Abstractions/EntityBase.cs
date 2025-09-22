using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.Shared.Abstractions;

public abstract class EntityBase : IEntityBase
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}