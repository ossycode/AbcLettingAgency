namespace AbcLettingAgency.Shared.Abstractions;

public interface IErrorEventId
{
    Guid? EventId { get; }
}
