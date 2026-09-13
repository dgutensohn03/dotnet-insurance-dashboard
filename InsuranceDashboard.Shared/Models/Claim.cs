using System.ComponentModel.DataAnnotations;

namespace InsuranceDashboard.Shared.Models;

public class Claim
{
    public int Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string ClaimNumber { get; set; } = string.Empty;

    [Required]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Open";

    [Range(0, 1_000_000)]
    public decimal Amount { get; set; }

    public DateOnly LossDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public bool IsArchived { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
