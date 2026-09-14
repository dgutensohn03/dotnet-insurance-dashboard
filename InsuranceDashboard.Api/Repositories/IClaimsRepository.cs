using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public interface IClaimsRepository
{
    Task<IReadOnlyList<Claim>> GetAllAsync();
    Task<Claim?> GetByIdAsync(int id);
    Task<Claim> AddAsync(Claim claim);
    Task<Claim?> UpdateAsync(int id, Claim claim);
    Task<Claim?> ArchiveAsync(int id);
    Task<Claim?> RestoreAsync(int id);
}
