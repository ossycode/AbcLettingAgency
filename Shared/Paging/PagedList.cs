namespace AbcLettingAgency.Shared.Paging;

public sealed class PagedList<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int Total { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)Total / Math.Max(1, PageSize));
}
