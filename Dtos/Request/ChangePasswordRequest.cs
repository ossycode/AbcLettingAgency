namespace AbcLettingAgency.Dtos.Request;

public sealed class ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = default!;
    public string NewPassword { get; init; } = default!;
}
