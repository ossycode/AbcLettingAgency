using AbcLettingAgency.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AbcLettingAgency.Data.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a global filter e => !e.IsDeleted for every entity implementing ISoftDelete.
    /// </summary>
    public static void AddSoftDeleteGlobalFilter(this ModelBuilder builder)
    {
        var softDeleteClrs = builder.Model.GetEntityTypes()
                   .Where(t => !t.IsOwned() && typeof(ISoftDelete).IsAssignableFrom(t.ClrType))
                   .Select(t => t.ClrType)
                   .Distinct();

        foreach (var clr in softDeleteClrs)
        {
            var p = Expression.Parameter(clr, "e");
            var body = Expression.Equal(
                Expression.Property(p, nameof(ISoftDelete.IsDeleted)),
                Expression.Constant(false));
            builder.Entity(clr).HasQueryFilter(Expression.Lambda(body, p));
        }
    }

    /// <summary>
    /// Adds a global filter for all IAgencyOwned entities:
    ///   if ctx._agencyId != null => e.AgencyId == ctx._agencyId
    ///   else (platform / no agency) => false  (no rows)
    ///
    /// This binds the filter to the specific DbContext instance so each request
    /// gets the right agency scoping. It expects AppDbContext to have a private
    /// field named "_agencyId" of type long?.
    /// </summary>
    public static void AddAgencyOwnedGlobalFilter(this ModelBuilder builder, AppDbContext ctx)
    {
        // ctx.CurrentAgencyId (long?)
        var ctxConst = Expression.Constant(ctx);
        var aidAccess = Expression.Property(ctxConst, nameof(AppDbContext.CurrentAgencyId));

        Console.WriteLine(aidAccess);

        // All IAgencyOwned entities
        var agencyOwnedClrs = builder.Model.GetEntityTypes()
            .Where(t => !t.IsOwned() && typeof(IAgencyOwned).IsAssignableFrom(t.ClrType))
            .Select(t => t.ClrType)
            .Distinct()
            .ToArray();

        foreach (var clr in agencyOwnedClrs)
        {
            var param = Expression.Parameter(clr, "e");                          // e
            var agencyProp = Expression.Property(param, nameof(IAgencyOwned.AgencyId)); // e.AgencyId

            var hasAid = Expression.NotEqual(aidAccess, Expression.Constant(null, typeof(long?)));
            var eq = Expression.Equal(agencyProp, Expression.Convert(aidAccess, typeof(long)));

            // _agencyId != null ? e.AgencyId == _agencyId : false
            var body = Expression.Condition(hasAid, eq, Expression.Constant(false));
            var lambda = Expression.Lambda(body, param);

            builder.Entity(clr).HasQueryFilter(lambda);
        }
    }

    /// <summary>
    /// Maps PostgreSQL xmin as a concurrency token for all non-owned entities.
    /// </summary>
    public static void UsePostgresXminConcurrencyTokens(this ModelBuilder builder)
    {
        var clrs = builder.Model.GetEntityTypes()
                .Where(t => !t.IsOwned())
                .Select(t => t.ClrType)
                .Distinct();

        foreach (var clr in clrs)
        {
            builder.Entity(clr)
                .Property<uint>("xmin")
                .IsRowVersion()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }

}


//// Global filter for all entities that have IsDeleted
//foreach (var entityType in builder.Model.GetEntityTypes())
//{
//    if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
//    {
//        var param = Expression.Parameter(entityType.ClrType, "e");
//        var prop = Expression.Property(param, nameof(ISoftDelete.IsDeleted));
//        var notDeleted = Expression.Lambda(
//            Expression.Equal(prop, Expression.Constant(false)),
//            param
//        );
//        builder.Entity(entityType.ClrType).HasQueryFilter(notDeleted);
//    }
//}

//// GLOBAL QUERY FILTERS FOR IAgencyOwned

//var agencyClrTypes = builder.Model.GetEntityTypes()
//    .Where(t => !t.IsOwned() && typeof(IAgencyOwned).IsAssignableFrom(t.ClrType))
//    .Select(t => t.ClrType)
//    .ToArray();

//foreach (var clr in agencyClrTypes)
//{
//    var param = Expression.Parameter(clr, "e");
//    var agencyProp = Expression.Property(param, nameof(IAgencyOwned.AgencyId));

//    // this._agencyId (long?)
//    var aidField = Expression.Field(Expression.Constant(this), "_agencyId");
//    var aidNotNull = Expression.NotEqual(aidField, Expression.Constant(null, typeof(long?)));
//    var eq = Expression.Equal(agencyProp, Expression.Convert(aidField, typeof(long)));

//    // If _agencyId is present: e.AgencyId == _agencyId; else: FALSE (no rows).
//    var body = Expression.Condition(aidNotNull, eq, Expression.Constant(false));
//    builder.Entity(clr).HasQueryFilter(Expression.Lambda(body, param));
//}


//// ADDING xmin COLUMN TYPE 
//foreach (var et in builder.Model.GetEntityTypes().Where(t => !t.IsOwned()))
//{
//    builder.Entity(et.ClrType)
//        .Property<uint>("xmin")
//         .IsRowVersion()
//        .HasColumnName("xmin")
//        .HasColumnType("xid")
//        .ValueGeneratedOnAddOrUpdate()
//        .IsConcurrencyToken();
//}