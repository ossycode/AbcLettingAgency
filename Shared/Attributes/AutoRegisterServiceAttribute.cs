

namespace AbcLettingAgency.Shared.Attributes;

[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class AutoRegisterServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public AutoRegisterServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        => Lifetime = lifetime;
}
