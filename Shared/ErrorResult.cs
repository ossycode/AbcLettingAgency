namespace AbcLettingAgency.Shared;

public class ErrorResult
{
    public required string Message { get; set; }
    public string? Detail { get; set; }
    public IList<Error>? Errors { get; set; }
    public Guid? EventId { get; set; }
}

public class Error
{
    public string? Key { get; }
    public string Message { get; }
    public Error(string key, string message)
    {
        Key = string.IsNullOrWhiteSpace(key) ? null : key;
        Message = message;
    }
}
