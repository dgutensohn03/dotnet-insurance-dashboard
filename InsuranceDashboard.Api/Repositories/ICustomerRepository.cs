using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync();
}
