using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenants;

public static class TenantErrors
{
    public static AppError NotFound(long id) =>
        new("Tenant.NotFound", $"Tenant '{id}' not found.", ErrorType.NotFound, id.ToString());

    public static AppError DuplicateEmail(string email) =>
        new("Tenant.DuplicateEmail", $"Email '{email}' is already in use.", ErrorType.Conflict, "Email");

    public static AppError DuplicatePhone(string phone) =>
        new("Tenant.DuplicatePhone", $"Phone '{phone}' is already in use.", ErrorType.Conflict, "Phone");

    public static AppError HasActiveLinks(long id) =>
        new("Tenant.HasActiveLinks", $"Tenant '{id}' is linked to active tenancies.", ErrorType.Conflict, id.ToString());

    public static AppError AlreadyDeleted(long id) =>
        new("Tenant.AlreadyDeleted", $"Tenant '{id}' is already deleted.", ErrorType.Conflict, id.ToString());
}
