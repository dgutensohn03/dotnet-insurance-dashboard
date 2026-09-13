using InsuranceDashboard.Client.Services;
using InsuranceDashboard.Shared.Models;

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
    }

    [Fact]
    public async Task Summary_metrics_are_derived_from_portfolio_data()
    {
        var service = new DemoInsuranceDataService();
        var summary = await service.GetSummaryAsync();
        Assert.True(summary.ActivePolicies > 500);
        Assert.True(summary.WrittenPremium > 0);
        Assert.True(summary.OpenClaimExposure > 0);
        Assert.True(summary.LossRatio > 0);
    }

    [Fact]
    public async Task Customer_can_be_created_updated_and_deleted()
    {
        var service = new DemoInsuranceDataService();
        var created = await service.AddCustomerAsync(new Customer { Name="Test Customer", Email="test@example.com", State="CO" });
        Assert.True(created.Id > 0);
        created.Name = "Updated Customer";
        await service.UpdateCustomerAsync(created);
        Assert.Contains(await service.GetCustomersAsync(), c => c.Id == created.Id && c.Name == "Updated Customer");
        await service.DeleteCustomerAsync(created.Id);
        Assert.DoesNotContain(await service.GetCustomersAsync(), c => c.Id == created.Id);
    }
}
