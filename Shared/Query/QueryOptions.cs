namespace AbcLettingAgency.Shared.Query;

public sealed class QueryOptions
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }      
    public bool SortDesc { get; init; }
    public string? Search { get; init; }      
    public string[] SearchFields { get; init; } = Array.Empty<string>(); // which string fields to search
}

public sealed class FilterRule
{
    // e.g. Field="City", Op="contains|eq|gt|lt", Value="London"
    public required string Field { get; init; }
    public required string Op { get; init; }
    public required string Value { get; init; }
}
