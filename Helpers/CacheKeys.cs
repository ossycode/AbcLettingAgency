using Microsoft.Extensions.Caching.Hybrid;

namespace AbcLettingAgency.Helpers;

public static class CacheKeys
{
    public static string UserAgency(Guid userId)
    {
        return $"user_agency:{userId}";
    }
}

public interface IAgencyUserEvents
{
    Task EvictUserAgencyAsync(Guid userId);
}

//Call EvictUserAgencyAsync whenever you add/remove/deactivate an AgencyUser.
public sealed class AgencyUserEvents(HybridCache cache) : IAgencyUserEvents
{
    public async Task EvictUserAgencyAsync(Guid userId)
        => await cache.RemoveAsync(CacheKeys.UserAgency(userId));
}