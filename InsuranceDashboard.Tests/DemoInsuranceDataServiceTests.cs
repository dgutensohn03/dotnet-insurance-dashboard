using InsuranceDashboard.Client.Services;
using InsuranceDashboard.Shared.Models;
using Xunit;

namespace InsuranceDashboard.Tests;

public class DemoInsuranceDataServiceTests
{
    [Fact]
    public async Task Portfolio_contains_case_study_claim_with_expected_values()
    {
        var service = new DemoInsuranceDataService();
        var claim = (await service.GetClaimsAsync()).Single(c => c.ClaimNumber == "CLM-10482");
        Assert.Equal("POL-48392", claim.PolicyNumber);
        Assert.Equal(42850m, claim.Amount);
        Assert.Equal("Investigating", claim.Status);
        Assert.False(claim.IsArchived);
    }

    [Fact]
    public async Task Summary_metrics_are_derived_from_active_portfolio_data()
    {
        var service = new DemoInsuranceDataService();
        var before = await service.GetSummaryAsync();
        var claim = (await service.GetClaimsAsync()).First(c => c.Status is "Open" or "Investigating");
        await service.ArchiveClaimAsync(claim.Id);
        var after = await service.GetSummaryAsync();
        Assert.True(before.ActivePolicies > 500);
        Assert.True(before.WrittenPremium > 0);
        Assert.True(before.OpenClaimExposure > 0);
        Assert.True(after.OpenClaimExposure <= before.OpenClaimExposure);
    }

    [Fact]
    public async Task Customer_can_be_created_updated_archived_and_restored()
    {
        var service = new DemoInsuranceDataService();
        var created = await service.AddCustomerAsync(new Customer { Name="Test Customer", Email="test@example.com", State="CO" });
        Assert.True(created.Id > 0);
        created.Name = "Updated Customer";
        await service.UpdateCustomerAsync(created);
        var archived = await service.ArchiveCustomerAsync(created.Id);
        Assert.True(archived.IsArchived);
        Assert.NotNull(archived.ArchivedAt);
        var restored = await service.RestoreCustomerAsync(created.Id);
        Assert.False(restored.IsArchived);
        Assert.Null(restored.ArchivedAt);
        Assert.Contains(await service.GetCustomersAsync(), c => c.Id == created.Id && c.Name == "Updated Customer");
    }

    [Fact]
    public async Task Claim_archive_is_recoverable()
    {
        var service = new DemoInsuranceDataService();
        var claim = (await service.GetClaimsAsync()).Single(c => c.ClaimNumber == "CLM-10482");
        await service.ArchiveClaimAsync(claim.Id);
        Assert.True(claim.IsArchived);
        await service.RestoreClaimAsync(claim.Id);
        Assert.False(claim.IsArchived);
    }
}
