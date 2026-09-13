using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Client.Services;

public interface IInsuranceDataService
{
    Task<IReadOnlyList<Policy>> GetPoliciesAsync();
    Task<IReadOnlyList<Claim>> GetClaimsAsync();
    Task<IReadOnlyList<Customer>> GetCustomersAsync();
    Task<DashboardSummary> GetSummaryAsync();
    Task<Claim> AddClaimAsync(Claim claim);
    Task<Customer> AddCustomerAsync(Customer customer);
    Task<Customer> UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(int id);
}
