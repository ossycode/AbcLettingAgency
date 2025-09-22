using Microsoft.AspNetCore.Identity;

namespace AbcLettingAgency.Shared.Exceptions;

public static class Errors
{
    public static class Auth
    {
        public static AppError Unauthorized(string? msg = null)
            => new("Auth.Unauthorized", msg ?? "Unauthorized", ErrorType.Unauthorized);

        public static AppError InvalidCredentials()
            => new("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized);

        public static AppError RefreshTokenMissing()
            => new("Auth.RefreshTokenMissing", "Refresh token is missing.", ErrorType.Validation);

        public static AppError RefreshTokenInvalid()
            => new("Auth.RefreshTokenInvalid", "Unable to retrieve user for refresh token.", ErrorType.Unauthorized);

        public static AppError RefreshTokenExpired()
            => new("Auth.RefreshTokenExpired", "Refresh token is expired.", ErrorType.Unauthorized);
    }

    public static class User
    {
        public static AppError AlreadyExists(string email)
            => new("User.AlreadyExists", $"A user with email '{email}' already exists.", ErrorType.Conflict, key: "email");

        public static AppError NotFound(string? key = null)
            => new("User.NotFound", "User not found.", ErrorType.NotFound, key);

        public static AppError InvalidPassword()
            => new("User.InvalidPassword", "Invalid password.", ErrorType.Validation, key: "password");
    }

    public static class Identity
    {
        public static IEnumerable<AppError> FromIdentity(IdentityResult result, string prefix = "Identity")
            => result.Errors.Select(e =>
                new AppError($"{prefix}.{e.Code}", e.Description, ErrorType.Validation));
    }
}

