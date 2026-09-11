using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public interface IPolicyRepository
{
    Task<IReadOnlyList<Policy>> GetAllAsync();
    Task<Policy?> GetByIdAsync(int id);
}
