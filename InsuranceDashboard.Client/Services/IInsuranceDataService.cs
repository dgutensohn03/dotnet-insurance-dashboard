using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Client.Services;

public interface IInsuranceDataService
{
    Task<IReadOnlyList<Policy>> GetPoliciesAsync();
    Task<IReadOnlyList<Claim>> GetClaimsAsync();
    Task<IReadOnlyList<Customer>> GetCustomersAsync();
    Task<DashboardSummary> GetSummaryAsync();
    Task<Claim> AddClaimAsync(Claim claim);
    Task<Claim> UpdateClaimAsync(Claim claim);
    Task<Claim> ArchiveClaimAsync(int id);
    Task<Claim> RestoreClaimAsync(int id);
    Task<Policy> UpdatePolicyAsync(Policy policy);
    Task<Customer> AddCustomerAsync(Customer customer);
    Task<Customer> UpdateCustomerAsync(Customer customer);
    Task<Customer> ArchiveCustomerAsync(int id);
    Task<Customer> RestoreCustomerAsync(int id);
}
