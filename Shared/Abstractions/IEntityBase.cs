namespace AbcLettingAgency.Shared.Abstractions;

public interface IEntityBase
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime? DeletedAtUtc { get; set; }
    Guid? UserDeletedId { get; set; }
    Guid? UserUpdatedId { get; set; }
}
