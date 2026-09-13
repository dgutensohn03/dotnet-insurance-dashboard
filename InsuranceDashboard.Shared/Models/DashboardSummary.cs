namespace InsuranceDashboard.Shared.Models;

public record DashboardSummary(
    int ActivePolicies,
    int OpenClaims,
    decimal WrittenPremium,
    decimal OpenClaimExposure,
    decimal LossRatio);
