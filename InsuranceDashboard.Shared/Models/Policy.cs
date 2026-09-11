namespace InsuranceDashboard.Shared.Models;

public record Policy(
    int Id,
    string PolicyNumber,
    string CustomerName,
    string Type,
    decimal Premium,
    bool Active,
    DateOnly EffectiveDate);
