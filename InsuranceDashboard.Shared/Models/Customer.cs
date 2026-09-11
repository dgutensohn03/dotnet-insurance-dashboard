namespace InsuranceDashboard.Shared.Models;

public record Customer(
    int Id,
    string Name,
    string Email,
    string State,
    int PolicyCount,
    decimal TotalPremium);
