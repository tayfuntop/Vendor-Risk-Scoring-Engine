using Microsoft.EntityFrameworkCore;
using VendorRisk.Application.Interfaces;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Infrastructure.Persistence.PostgreSql.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly PostgresContext _context;

    public VendorRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<VendorProfile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<VendorProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VendorProfiles
            .OrderBy(v => v.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(VendorProfile vendor, CancellationToken cancellationToken = default)
    {
        await _context.VendorProfiles.AddAsync(vendor, cancellationToken);
    }

    public Task UpdateAsync(VendorProfile vendor, CancellationToken cancellationToken = default)
    {
        _context.VendorProfiles.Update(vendor);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await GetByIdAsync(id, cancellationToken);
        if (vendor != null)
        {
            _context.VendorProfiles.Remove(vendor);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
