using System.Net.Http.Json;
using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Client.Services;

public sealed class ApiInsuranceDataService(
    HttpClient httpClient,
    IConfiguration configuration) : IInsuranceDataService
{
    private readonly string _apiBaseUrl =
        configuration["ApiBaseUrl"]?.TrimEnd('/') ?? "http://localhost:5159";

    public async Task<IReadOnlyList<Policy>> GetPoliciesAsync() =>
        await httpClient.GetFromJsonAsync<List<Policy>>($"{_apiBaseUrl}/api/policies") ?? [];

    public async Task<IReadOnlyList<Claim>> GetClaimsAsync() =>
        await httpClient.GetFromJsonAsync<List<Claim>>($"{_apiBaseUrl}/api/claims") ?? [];

    public async Task<IReadOnlyList<Customer>> GetCustomersAsync() =>
        await httpClient.GetFromJsonAsync<List<Customer>>($"{_apiBaseUrl}/api/customers") ?? [];

    public async Task<DashboardSummary> GetSummaryAsync() =>
        await httpClient.GetFromJsonAsync<DashboardSummary>($"{_apiBaseUrl}/api/summary")
        ?? new DashboardSummary(0, 0, 0, 0);

    public async Task<Claim> AddClaimAsync(Claim claim)
    {
        var response = await httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/claims", claim);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Claim>()
            ?? throw new InvalidOperationException("API did not return a claim.");
    }
}
