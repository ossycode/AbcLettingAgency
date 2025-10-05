namespace AbcLettingAgency.Shared.Exceptions;

public sealed class BatchAbortException(AppError error) : Exception(error.Message)
{
    public AppError Error { get; } = error;
}
