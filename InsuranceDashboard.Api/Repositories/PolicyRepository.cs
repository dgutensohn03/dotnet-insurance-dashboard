using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public sealed class PolicyRepository : IPolicyRepository
{
    private static readonly List<Policy> Policies =
    [
        new(1, "AUTO-1001", "Avery Johnson", "Auto", 125.00m, true, new DateOnly(2026, 1, 15)),
        new(2, "HOME-2002", "Morgan Lee", "Home", 240.00m, false, new DateOnly(2025, 11, 1)),
        new(3, "AUTO-3003", "Jordan Smith", "Auto", 175.00m, true, new DateOnly(2026, 4, 10)),
        new(4, "HOME-4004", "Casey Brown", "Home", 310.00m, true, new DateOnly(2026, 3, 5)),
        new(5, "AUTO-5005", "Riley Garcia", "Auto", 198.00m, true, new DateOnly(2026, 6, 12)),
        new(6, "RENTERS-6006", "Taylor Wilson", "Renters", 42.00m, true, new DateOnly(2026, 7, 1))
    ];

    public Task<IReadOnlyList<Policy>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Policy>>(Policies);

    public Task<Policy?> GetByIdAsync(int id) =>
        Task.FromResult(Policies.FirstOrDefault(p => p.Id == id));
}
