using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.TenantManagement.Tenants.Services;

public class TenantService(
    IEntityServiceDependencies deps,
    IEntityServiceFactory entityService)
    : BaseEntityService<Tenant>(deps), ITenantService
{
    private readonly IEntityServiceFactory _entityService = entityService;

    public async Task<Result<TenantDto>> CreateAsync(CreateTenantRequest req, CancellationToken token)
    {
        var first = (req.FirstName ?? string.Empty).Trim();
        var last = (req.LastName ?? string.Empty).Trim();
        var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        var phone = (req.Phone ?? string.Empty).Trim();
        var email2 = req.SecondEmail?.Trim().ToLowerInvariant();
        var phone2 = req.SecondPhone?.Trim();

        if (await EmailInUseAsync(email, excludeTenantId: null, token))
            return TenantErrors.DuplicateEmail(email);
        if (await PhoneInUseAsync(phone, excludeTenantId: null, token))
            return TenantErrors.DuplicatePhone(phone);

        var entity = new Tenant
        {
            FirstName = first,
            LastName = last,
            Email = email,
            SecondEmail = email2,
            Phone = phone,
            SecondPhone = phone2,
            Status = TenantStatus.ACTIVE,
            Notes = req.Notes,
            IsDeleted = false,
        };

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            await base.CreateAsync(entity, token);
        }, token);


        var dto = Mappers.TenantToDto(entity);

        return dto;
    }

    public async Task<Result<TenantDto>> UpdateAsync(long tenantId, UpdateTenantRequest req, CancellationToken token)
    {
        var tenant = await _entityService.For<Tenant>()
            .GetAll()
            .FirstOrDefaultAsync(t => t.Id == tenantId, token);

        if (tenant is null)
            return TenantErrors.NotFound(tenantId);

        var first = (req.FirstName ?? string.Empty).Trim();
        var last = (req.LastName ?? string.Empty).Trim();
        var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        var phone = (req.Phone ?? string.Empty).Trim();
        var email2 = req.SecondEmail?.Trim().ToLowerInvariant();
        var phone2 = req.SecondPhone?.Trim();

        if (!email.Equals(tenant.Email, StringComparison.OrdinalIgnoreCase) &&
            await EmailInUseAsync(email, tenantId, token))
            return TenantErrors.DuplicateEmail(email);

        if (!phone.Equals(tenant.Phone, StringComparison.OrdinalIgnoreCase) &&
            await PhoneInUseAsync(phone, tenantId, token))
            return TenantErrors.DuplicatePhone(phone);

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            tenant.FirstName = first;
            tenant.LastName = last;
            tenant.Email = email;
            tenant.SecondEmail = email2;
            tenant.Phone = phone;
            tenant.SecondPhone = phone2;
            tenant.Status = req.Status;
            tenant.Notes = req.Notes;

            await UpdateAsync(tenant);
        }, token);

        var dto = Mappers.TenantToDto(tenant);

        return dto;
    }

    public async Task<Result> DeleteAsync(long tenantId, CancellationToken token)
    {
        // Hard delete only if no links/docs/updates
        var tenant = await _entityService.For<Tenant>()
            .GetAll()
            .Include(t => t.TenancyLinks)
            .Include(t => t.Documents)
            .Include(t => t.Updates)
            .FirstOrDefaultAsync(t => t.Id == tenantId, token);

        if (tenant is null)
            return Result.Failure(TenantErrors.NotFound(tenantId));

        var hasAnyLinks = tenant.TenancyLinks.Any();
        if (hasAnyLinks)
            return Result.Failure(TenantErrors.HasActiveLinks(tenantId));

        if (tenant.Documents.Any() || tenant.Updates.Any())
        {
            // choose your policy; here we block hard delete if there’s history
            return Result.Failure(new AppError(
                "Tenant.HasHistory",
                "Tenant has linked documents or updates. Remove them first or use soft delete.",
                ErrorType.Conflict,
                tenantId.ToString()));
        }

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            await base.DeleteAsync(tenant, token);
        }, token);

        return Result.Success();
    }

    private async Task<bool> EmailInUseAsync(string email, long? excludeTenantId, CancellationToken token)
    {
        var q = _entityService.For<Tenant>()
            .GetAll()
            .Where(t => !t.IsDeleted && t.Email.ToLower() == email.ToLower());

        if (excludeTenantId.HasValue)
            q = q.Where(t => t.Id != excludeTenantId.Value);

        return await q.AnyAsync(token);
    }

    private async Task<bool> PhoneInUseAsync(string phone, long? excludeTenantId, CancellationToken token)
    {
        var q = _entityService.For<Tenant>()
            .GetAll()
            .Where(t => !t.IsDeleted && t.Phone == phone);

        if (excludeTenantId.HasValue)
            q = q.Where(t => t.Id != excludeTenantId.Value);

        return await q.AnyAsync(token);
    }

}
