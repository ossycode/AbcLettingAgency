using AbcLettingAgency.Data.Extensions;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using AbcLettingAgency.Helpers;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security;
using static System.ArgumentNullException;

namespace AbcLettingAgency.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser current, IAmbientAgency ambient) : 
    IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{

    private readonly ICurrentUser _current = current;
    private readonly long? _agencyId = current.AgencyIdClaim;
    private readonly IAmbientAgency _ambient = ambient;
    internal long? CurrentAgencyId => current.AgencyIdClaim ?? _ambient.Current;

    public bool BypassTenantRules { get; set; }

    public DbSet<Landlord> Landlords => Set<Landlord>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Tenancy> Tenancies => Set<Tenancy>();
    public DbSet<TenancyTenant> TenancyTenants => Set<TenancyTenant>();
    public DbSet<RentCharge> RentCharges => Set<RentCharge>();
    public DbSet<RentReceipt> RentReceipts => Set<RentReceipt>();
    public DbSet<ClientLedger> ClientLedgers => Set<ClientLedger>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<MaintenanceJob> MaintenanceJobs => Set<MaintenanceJob>();
    public DbSet<Update> Updates => Set<Update>();
    public DbSet<Document> Documents => Set<Document>();

    public DbSet<Agency> Agencies => Set<Agency>();
    public DbSet<AgencyGroup> AgencyGroups => Set<AgencyGroup>();
    public DbSet<AgencyGroupMembership> AgencyGroupMemberships => Set<AgencyGroupMembership>();
    public DbSet<AgencyUser> AgencyUsers => Set<AgencyUser>();
    public DbSet<BillingAccount> BillingAccounts => Set<BillingAccount>();
    public DbSet<BillingSubscription> BillingSubscriptions => Set<BillingSubscription>();
    public DbSet<BillingSubscriptionItem> BillingSubscriptionItems => Set<BillingSubscriptionItem>();
    public DbSet<BillingInvoice> BillingInvoices => Set<BillingInvoice>();
    public DbSet<AgencyConfiguration> AgencyConfigurations => Set<AgencyConfiguration>();


    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.HasDefaultSchema("public");

        //builder.Entity<EntityBase>().HasQueryFilter(t => !t.IsDeleted);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        builder.AddSoftDeleteGlobalFilter();
        //builder.AddAgencyOwnedGlobalFilter(this);
        builder.UsePostgresXminConcurrencyTokens();

        foreach (var et in builder.Model.GetEntityTypes()
                 .Where(t => !t.IsOwned() && typeof(IAgencyOwned).IsAssignableFrom(t.ClrType))
                 .Select(t => t.ClrType))
        {
            // Call a generic helper so we can write a strongly-typed lambda
            GetType()
                .GetMethod(nameof(ApplyAgencyFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(et)
                .Invoke(this, new object[] { builder });
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        configurationBuilder.Properties<decimal?>().HavePrecision(18, 2);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var uid = _current.UserId;
        var isPlatform = _current.IsPlatform();

        var needsTenant = !BypassTenantRules &&
                  ChangeTracker.Entries()
                  .Any(e => e.State is not (EntityState.Unchanged or EntityState.Detached)
                            && e.Entity is IAgencyOwned);

        long? aid = null;

        if (needsTenant && !isPlatform)
        {
            aid = _agencyId ?? await _current.GetAgencyId();
        }

        foreach (var e in ChangeTracker.Entries())
        {
            // Soft delete
            if (e.State == EntityState.Deleted && e.Entity is ISoftDelete)
            {
                e.State = EntityState.Modified;
                e.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
                e.CurrentValues["UpdatedAt"] = now;
                e.CurrentValues["DeletedAtUtc"] = now;
                if (uid.HasValue && e.Metadata.FindProperty("UserDeletedId") != null)
                    e.CurrentValues["UserDeletedId"] = uid.Value;

                // Cross-tenant delete guard for agency users
                if (!isPlatform && e.Entity is IAgencyOwned delOwned && aid.HasValue && delOwned.AgencyId != aid.Value)
                    throw new SecurityException("Cross-tenant delete blocked.");

                continue;
            }

            // Timestamps/user
            if (e.Metadata.FindProperty("UpdatedAt") != null && e.State is EntityState.Added or EntityState.Modified)
                e.CurrentValues["UpdatedAt"] = now;
            if (e.Metadata.FindProperty("CreatedAt") != null && e.State == EntityState.Added)
                e.CurrentValues["CreatedAt"] = now;
            if (uid.HasValue && e.Metadata.FindProperty("UserUpdatedId") != null && e.State is EntityState.Added or EntityState.Modified)
                e.CurrentValues["UserUpdatedId"] = uid.Value;

            if (!BypassTenantRules && e.Entity is IAgencyOwned owned)
            {

                if (e.State == EntityState.Added)
                {
                    if (!isPlatform)
                    {
                        // Agency users: auto-fill & enforce same-tenant
                        if (owned.AgencyId <= 0) owned.AgencyId = aid!.Value;
                        else if (owned.AgencyId != aid) throw new SecurityException("Cross-tenant create blocked.");
                    }
                    else
                    {
                        // Platform must set AgencyId explicitly when creating
                        if (owned.AgencyId <= 0)
                            throw new SecurityException("Platform must set AgencyId explicitly.");
                    }
                }
                else if (e.State == EntityState.Modified)
                {
                    // Prevent AgencyId tampering
                    e.Property(nameof(IAgencyOwned.AgencyId)).IsModified = false;

                    // Agency users: enforce tenant on update
                    if (!isPlatform && aid.HasValue && owned.AgencyId != aid.Value)
                        throw new SecurityException("Cross-tenant update blocked.");
                }
            }
        }

        return await base.SaveChangesAsync(ct);
    }


    private void ApplyAgencyFilter<TEntity>(ModelBuilder b) where TEntity : class, IAgencyOwned
    {
        // IMPORTANT: reference the instance property (CurrentAgencyId) directly
        b.Entity<TEntity>().HasQueryFilter(e =>
             BypassTenantRules || (CurrentAgencyId != null && e.AgencyId == CurrentAgencyId));
    }
}

