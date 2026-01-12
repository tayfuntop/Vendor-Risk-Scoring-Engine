using Microsoft.EntityFrameworkCore;
using VendorRisk.Application.Interfaces;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Infrastructure.Persistence.PostgreSql.Repositories;

public class RiskAssessmentRepository : IRiskAssessmentRepository
{
    private readonly PostgresContext _context;

    public RiskAssessmentRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<RiskAssessment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .Include(r => r.Vendor)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<RiskAssessment>> GetByVendorIdAsync(int vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .Where(r => r.VendorId == vendorId)
            .OrderByDescending(r => r.AssessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<RiskAssessment?> GetLatestByVendorIdAsync(int vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .Where(r => r.VendorId == vendorId)
            .OrderByDescending(r => r.AssessedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(RiskAssessment assessment, CancellationToken cancellationToken = default)
    {
        await _context.RiskAssessments.AddAsync(assessment, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
