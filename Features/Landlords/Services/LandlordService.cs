using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.Landlords.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.Landlords.Services;

public class LandlordService(IEntityServiceDependencies deps, IEntityServiceFactory entityService)
    : BaseEntityService<Landlord>(deps), ILandlordService
{
    private readonly IEntityServiceFactory _entityService = entityService;
    public async Task<Result> CreateAsync(CreateLandlordRequest req, CancellationToken ct)
    {
        var v = Validate(req);
        if (v.Count > 0) return Result.Failure(LandlordErrors.Validation(v));

        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var exists = await Db.Landlords.AsNoTracking().AnyAsync(x => x.Email == req.Email, ct);
            if (exists) return Result.Failure(LandlordErrors.EmailAlreadyExists(req.Email));
        }

        var entity = new Landlord
        {
            Name = req.Name.Trim(),
            Email = req.Email?.Trim(),
            Phone = req.Phone?.Trim(),
            Address = req.Address?.Trim(),
            BankIban = req.BankIban?.Trim(),
            BankSort = req.BankSort?.Trim(),
            Notes = req.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await CreateAsync(entity, ct);

        return Result.Success();
    }
    public async Task<Result> UpdateAsync(string id, UpdateLandlordRequest req, CancellationToken ct)
    {
        var landlord = await _entityService.For<Landlord>().GetByIdAsync(id, ct);


        if (landlord is null) return Result.Failure(LandlordErrors.NotFound(id));

        var v = Validate(req);
        if (v.Count > 0) return Result.Failure(LandlordErrors.Validation(v));

        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var clash = await _entityService.For<Landlord>().GetAll()
                .AnyAsync(x => x.Email == req.Email && x.Id != id, ct);
            if (clash) return Result.Failure(LandlordErrors.EmailAlreadyExists(req.Email));
        }

        if (req.Name is { Length: > 0 }) landlord.Name = req.Name.Trim();
        if (req.Email is { Length: > 0 }) landlord.Email = req.Email.Trim();
        if (req.Phone is { Length: > 0 }) landlord.Phone = req.Phone.Trim();
        if (req.Address is { Length: > 0 }) landlord.Address = req.Address.Trim();
        if (req.BankIban is { Length: > 0 }) landlord.BankIban = req.BankIban.Trim();
        if (req.BankSort is { Length: > 0 }) landlord.BankSort = req.BankSort.Trim();
        if (req.Notes is not null) landlord.Notes = req.Notes.Trim();

        await UpdateAsync(landlord, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken ct)
    {
        var landload = await _entityService.For<Landlord>().GetAll()
            .Select(l => new
            {
                Entity = l,
                HasDeps = l.Properties.Any() || l.Tenancies.Any() || l.Invoices.Any()
            })
            .FirstOrDefaultAsync(x => x.Entity.Id == id, ct);

        if (landload is null) return Result.Failure(LandlordErrors.NotFound(id));
        if (landload.HasDeps) return Result.Failure(LandlordErrors.HasLinkedEntities());

        await DeleteAsync(landload.Entity, ct);
        return Result.Success();
    }

    private static List<KeyValuePair<string, string>> Validate(CreateLandlordRequest req)
    {
        var errs = new List<KeyValuePair<string, string>>();
        if (string.IsNullOrWhiteSpace(req.Name))
            errs.Add(new("name", "Name is required."));
        if (req.Name?.Length > 200) errs.Add(new("name", "Name cannot exceed 200 chars."));
        if (req.Email?.Length > 256) errs.Add(new("email", "Email cannot exceed 256 chars."));
        if (req.Phone?.Length > 50) errs.Add(new("phone", "Phone cannot exceed 50 chars."));
        if (req.Address?.Length > 400) errs.Add(new("address", "Address cannot exceed 400 chars."));
        if (req.BankIban?.Length > 34) errs.Add(new("bankIban", "IBAN too long."));
        if (req.BankSort?.Length > 20) errs.Add(new("bankSort", "Sort code too long."));
        return errs;
    }

    private static List<KeyValuePair<string, string>> Validate(UpdateLandlordRequest req)
    {
        var errs = new List<KeyValuePair<string, string>>();
        if (req.Name is { Length: > 200 }) errs.Add(new("name", "Name cannot exceed 200 chars."));
        if (req.Email is { Length: > 256 }) errs.Add(new("email", "Email cannot exceed 256 chars."));
        if (req.Phone is { Length: > 50 }) errs.Add(new("phone", "Phone cannot exceed 50 chars."));
        if (req.Address is { Length: > 400 }) errs.Add(new("address", "Address cannot exceed 400 chars."));
        if (req.BankIban is { Length: > 34 }) errs.Add(new("bankIban", "IBAN too long."));
        if (req.BankSort is { Length: > 20 }) errs.Add(new("bankSort", "Sort code too long."));
        return errs;
    }
}
