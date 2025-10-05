namespace AbcLettingAgency.Authorization;

public static class AppClaim
{
    public const string Permission = "permission";
    public const string Expiration = "exp";
    public const string AgencyId = "https://abcletting.app/claims/agency_id";
    public const string PlatformRole = "platform_role";
}
