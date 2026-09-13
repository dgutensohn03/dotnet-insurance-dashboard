using System.ComponentModel.DataAnnotations;

namespace InsuranceDashboard.Shared.Models;

public class Customer
{
    public Customer() { }

    public Customer(int id, string name, string email, string state, int policyCount, decimal totalPremium)
    {
        Id = id; Name = name; Email = email; State = state; PolicyCount = policyCount; TotalPremium = totalPremium;
    }

    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string State { get; set; } = "CO";

    public int PolicyCount { get; set; }
    public decimal TotalPremium { get; set; }
}
