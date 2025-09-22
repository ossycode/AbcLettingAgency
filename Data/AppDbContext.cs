using AbcLettingAgency.EntityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AbcLettingAgency.Data;


public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Landlord> Landlords => Set<Landlord>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Tenancy> Tenancies => Set<Tenancy>();
    public DbSet<RentCharge> RentCharges => Set<RentCharge>();
    public DbSet<RentReceipt> RentReceipts => Set<RentReceipt>();
    public DbSet<ClientLedger> ClientLedger => Set<ClientLedger>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<MaintenanceJob> MaintenanceJobs => Set<MaintenanceJob>();
    public DbSet<Update> Updates => Set<Update>();
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<AppUser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<AppUser>().HasIndex(u => u.RefreshToken).IsUnique();

        modelBuilder.Entity<Property>().HasIndex(x => x.Code).IsUnique();

        modelBuilder.Entity<Tenancy>().HasIndex(x => new { x.PropertyId, x.Status });
        modelBuilder.Entity<Tenancy>().HasIndex(x => new { x.TenantId, x.Status });
        modelBuilder.Entity<RentCharge>().HasIndex(x => new { x.TenancyId, x.DueDate, x.Status });
        modelBuilder.Entity<RentReceipt>().HasIndex(x => new { x.TenancyId, x.ReceivedAt });

        modelBuilder.Entity<Property>()
            .HasOne(p => p.Landlord)
            .WithMany(l => l.Properties)
            .HasForeignKey(p => p.LandlordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tenancy>()
            .HasOne(t => t.Property)
            .WithMany(p => p.Tenancies)
            .HasForeignKey(t => t.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tenancy>()
            .HasOne(t => t.Landlord)
            .WithMany(l => l.Tenancies)
            .HasForeignKey(t => t.LandlordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tenancy>()
            .HasOne(t => t.Tenant)
            .WithMany(te => te.Tenancies)
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RentReceipt>()
            .HasOne(r => r.Charge)
            .WithMany(c => c.Receipts)
            .HasForeignKey(r => r.ChargeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Update>()
            .HasOne(u => u.Property).WithMany(p => p.Updates)
            .HasForeignKey(u => u.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Update>()
            .HasOne(u => u.Tenancy).WithMany(t => t.Updates)
            .HasForeignKey(u => u.TenancyId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Update>()
            .HasOne(u => u.Landlord).WithMany(l => l.Updates)
            .HasForeignKey(u => u.LandlordId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Update>()
            .HasOne(u => u.Tenant).WithMany(t => t.Updates)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Invoice)
            .WithMany(i => i.Documents)
            .HasForeignKey(d => d.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tenancy>().Property(x => x.RentAmount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<Tenancy>().Property(x => x.CommissionPercent).HasColumnType("decimal(5,2)");
        modelBuilder.Entity<Tenancy>().Property(x => x.CommissionPercentTo15).HasColumnType("decimal(5,2)");
        modelBuilder.Entity<Tenancy>().Property(x => x.DepositAmount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<RentCharge>().Property(x => x.Amount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<RentCharge>().Property(x => x.CommissionDue).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<RentCharge>().Property(x => x.AmountAfterCommission).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<RentReceipt>().Property(x => x.Amount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<ClientLedger>().Property(x => x.Amount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<Invoice>().Property(x => x.NetAmount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<Invoice>().Property(x => x.VatAmount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<Invoice>().Property(x => x.GrossAmount).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<MaintenanceJob>().Property(x => x.Cost).HasColumnType("decimal(12,2)");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is AppUser au)
                {
                    au.CreatedAt = now;
                    au.UpdatedAt = now;
                }
                else if (entry.Metadata.FindProperty("CreatedAt") != null)
                {
                    entry.CurrentValues["CreatedAt"] = now;
                    entry.CurrentValues["UpdatedAt"] = now;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is AppUser au)
                    au.UpdatedAt = now;
                else if (entry.Metadata.FindProperty("UpdatedAt") != null)
                    entry.CurrentValues["UpdatedAt"] = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

