using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public sealed class ClaimsRepository : IClaimsRepository
{
    private static readonly List<Claim> Claims =
    [
        new() { Id = 1, ClaimNumber = "CLM-9001", PolicyNumber = "AUTO-1001", Status = "Open", Amount = 4200m, LossDate = new DateOnly(2026, 8, 12) },
        new() { Id = 2, ClaimNumber = "CLM-9002", PolicyNumber = "HOME-4004", Status = "Investigating", Amount = 12750m, LossDate = new DateOnly(2026, 8, 28) },
        new() { Id = 3, ClaimNumber = "CLM-9003", PolicyNumber = "AUTO-3003", Status = "Closed", Amount = 1800m, LossDate = new DateOnly(2026, 7, 14) },
        new() { Id = 4, ClaimNumber = "CLM-9004", PolicyNumber = "AUTO-5005", Status = "Open", Amount = 7350m, LossDate = new DateOnly(2026, 9, 2) }
    ];

    public Task<IReadOnlyList<Claim>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Claim>>(Claims.OrderByDescending(c => c.LossDate).ToList());

    public Task<Claim?> GetByIdAsync(int id) =>
        Task.FromResult(Claims.FirstOrDefault(c => c.Id == id));

    public Task<Claim> AddAsync(Claim claim)
    {
        claim.Id = Claims.Count == 0 ? 1 : Claims.Max(c => c.Id) + 1;
        Claims.Add(claim);
        return Task.FromResult(claim);
    }
}
