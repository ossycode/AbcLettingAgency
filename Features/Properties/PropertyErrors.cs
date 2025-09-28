using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.Properties;

public static class PropertyErrors
{
    public static AppError NotFound(string? id = null)
        => new(
            code: "Property.NotFound",
            message: "Property not found.",
            type: ErrorType.NotFound,
            key: id
        );

    public static AppError CodeAlreadyExists(string code)
        => new(
            code: "Property.CodeAlreadyExists",
            message: $"A property with code '{code}' already exists.",
            type: ErrorType.Conflict,
            key: "code"
        );

    public static AppError LandlordNotFound(string landlordId)
        => new(
            code: "Property.LandlordNotFound",
            message: $"Landlord '{landlordId}' was not found.",
            type: ErrorType.NotFound,
            key: "landlordId"
        );

    public static AppError HasLinkedEntities()
        => new(
            code: "Property.HasLinkedEntities",
            message: "Cannot delete a property with linked tenancies, maintenance jobs, invoices, updates or documents.",
            type: ErrorType.Conflict,
            key: "property"
        );

    public static AppError Validation(IEnumerable<KeyValuePair<string, string>> errors)
        => new(
            code: "Property.Validation",
            message: "One or more validation errors occurred.",
            type: ErrorType.Validation
        )
        {
            Details = errors.ToList()
        };
}

