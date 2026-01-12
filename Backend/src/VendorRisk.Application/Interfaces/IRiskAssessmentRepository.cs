using VendorRisk.Domain.Entities;

namespace VendorRisk.Application.Interfaces;

public interface IRiskAssessmentRepository
{
    Task<RiskAssessment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<RiskAssessment>> GetByVendorIdAsync(int vendorId, CancellationToken cancellationToken = default);
    Task<RiskAssessment?> GetLatestByVendorIdAsync(int vendorId, CancellationToken cancellationToken = default);
    Task AddAsync(RiskAssessment assessment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
