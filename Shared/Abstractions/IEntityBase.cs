namespace AbcLettingAgency.Shared.Abstractions;

public interface IEntityBase
{
    string Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
