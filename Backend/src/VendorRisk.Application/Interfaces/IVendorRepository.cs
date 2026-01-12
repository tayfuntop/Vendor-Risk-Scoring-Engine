using VendorRisk.Domain.Entities;

namespace VendorRisk.Application.Interfaces;

public interface IVendorRepository
{
    Task<VendorProfile?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<VendorProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(VendorProfile vendor, CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorProfile vendor, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
