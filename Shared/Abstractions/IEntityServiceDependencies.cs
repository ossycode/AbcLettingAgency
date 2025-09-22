using AbcLettingAgency.Data;

namespace AbcLettingAgency.Shared.Abstractions;

public interface IEntityServiceDependencies
{
    AppDbContext Db { get; }
    IChangeScope ChangeScope { get; }
    // add ILoggerFactory, ITimeProvider, etc. if you like
}

