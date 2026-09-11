using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Client.Services;

public sealed class DemoInsuranceDataService : IInsuranceDataService
{
    private readonly List<Policy> _policies =
    [
        new(1, "AUTO-1001", "Avery Johnson", "Auto", 125.00m, true, new DateOnly(2026, 1, 15)),
        new(2, "HOME-2002", "Morgan Lee", "Home", 240.00m, false, new DateOnly(2025, 11, 1)),
        new(3, "AUTO-3003", "Jordan Smith", "Auto", 175.00m, true, new DateOnly(2026, 4, 10)),
        new(4, "HOME-4004", "Casey Brown", "Home", 310.00m, true, new DateOnly(2026, 3, 5)),
        new(5, "AUTO-5005", "Riley Garcia", "Auto", 198.00m, true, new DateOnly(2026, 6, 12)),
        new(6, "RENTERS-6006", "Taylor Wilson", "Renters", 42.00m, true, new DateOnly(2026, 7, 1))
    ];

    private readonly List<Claim> _claims =
    [
        new() { Id = 1, ClaimNumber = "CLM-9001", PolicyNumber = "AUTO-1001", Status = "Open", Amount = 4200m, LossDate = new DateOnly(2026, 8, 12) },
        new() { Id = 2, ClaimNumber = "CLM-9002", PolicyNumber = "HOME-4004", Status = "Investigating", Amount = 12750m, LossDate = new DateOnly(2026, 8, 28) },
        new() { Id = 3, ClaimNumber = "CLM-9003", PolicyNumber = "AUTO-3003", Status = "Closed", Amount = 1800m, LossDate = new DateOnly(2026, 7, 14) },
        new() { Id = 4, ClaimNumber = "CLM-9004", PolicyNumber = "AUTO-5005", Status = "Open", Amount = 7350m, LossDate = new DateOnly(2026, 9, 2) }
    ];

    private readonly List<Customer> _customers =
    [
        new(1, "Avery Johnson", "avery@example.com", "CO", 1, 125m),
        new(2, "Morgan Lee", "morgan@example.com", "TX", 1, 240m),
        new(3, "Jordan Smith", "jordan@example.com", "AZ", 1, 175m),
        new(4, "Casey Brown", "casey@example.com", "CO", 1, 310m),
        new(5, "Riley Garcia", "riley@example.com", "NM", 1, 198m),
        new(6, "Taylor Wilson", "taylor@example.com", "CO", 1, 42m)
    ];

    public Task<IReadOnlyList<Policy>> GetPoliciesAsync() =>
        Task.FromResult<IReadOnlyList<Policy>>(_policies);

    public Task<IReadOnlyList<Claim>> GetClaimsAsync() =>
        Task.FromResult<IReadOnlyList<Claim>>(_claims.OrderByDescending(c => c.LossDate).ToList());

    public Task<IReadOnlyList<Customer>> GetCustomersAsync() =>
        Task.FromResult<IReadOnlyList<Customer>>(_customers);

    public Task<DashboardSummary> GetSummaryAsync()
    {
        var summary = new DashboardSummary(
            _policies.Count(p => p.Active),
            _claims.Count(c => c.Status is "Open" or "Investigating"),
            _policies.Where(p => p.Active).Sum(p => p.Premium),
            _claims.Where(c => c.Status != "Closed").Sum(c => c.Amount));

        return Task.FromResult(summary);
    }

    public Task<Claim> AddClaimAsync(Claim claim)
    {
        claim.Id = _claims.Count == 0 ? 1 : _claims.Max(c => c.Id) + 1;
        _claims.Add(claim);
        return Task.FromResult(claim);
    }
}
