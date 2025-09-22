using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.Landlords;

public static class LandlordErrors
{
    public static AppError NotFound(string? id = null)
        => new("Landlord.NotFound", "Landlord not found.", ErrorType.NotFound, key: id);

    public static AppError EmailAlreadyExists(string email)
        => new("Landlord.EmailAlreadyExists", $"A landlord with email '{email}' already exists.", ErrorType.Conflict, key: "email");

    public static AppError HasLinkedEntities()
        => new("Landlord.HasLinkedEntities",
               "Cannot delete a landlord with linked properties, tenancies or invoices.",
               ErrorType.Conflict,
               key: "landlord");

    public static AppError Validation(IEnumerable<KeyValuePair<string, string>> errors)
        => new("Landlord.Validation",
               "One or more validation errors occurred.",
               ErrorType.Validation)
        {
            Details = errors.ToList()
        };
}
