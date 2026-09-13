using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer?> UpdateAsync(int id, Customer customer);
    Task<Customer?> ArchiveAsync(int id);
    Task<Customer?> RestoreAsync(int id);
}
