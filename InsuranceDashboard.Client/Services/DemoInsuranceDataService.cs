using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Client.Services;

public sealed class DemoInsuranceDataService : IInsuranceDataService
{
    private readonly List<Customer> _customers;
    private readonly List<Policy> _policies;
    private readonly List<Claim> _claims;

    public DemoInsuranceDataService()
    {
        var first = new[] { "Avery","Morgan","Jordan","Casey","Riley","Taylor","Alex","Jamie","Cameron","Drew","Parker","Reese","Quinn","Sydney","Hayden" };
        var last = new[] { "Johnson","Lee","Smith","Brown","Garcia","Wilson","Martinez","Davis","Clark","Lewis","Walker","Hall","Young","King","Wright","Scott" };
        var states = new[] { "CO","TX","AZ","NM","UT","CA","WA","OR" };
        _customers = Enumerable.Range(1, 250).Select(i => new Customer(i, $"{first[(i-1)%first.Length]} {last[(i*3)%last.Length]}", $"customer{i:D3}@example.com", states[i%states.Length], 0, 0)).ToList();
        _customers[0].Name = "Jennifer Hart"; _customers[0].Email = "jennifer.hart@example.com"; _customers[0].State = "CO";

        var types = new[] { "Auto", "Home", "Renters", "Umbrella" };
        _policies = Enumerable.Range(1, 620).Select(i =>
        {
            var customer = _customers[(i-1)%_customers.Count]; var type = types[i%types.Length]; var premium = 65m + ((i*37)%390);
            return new Policy(i, $"{type.ToUpperInvariant()[..Math.Min(4,type.Length)]}-{48000+i}", customer.Name, type, premium, i%11!=0, new DateOnly(2025+(i%2), (i%12)+1, ((i*7)%27)+1));
        }).ToList();
        _policies[0] = new Policy(1, "POL-48392", "Jennifer Hart", "Auto", 286m, true, new DateOnly(2026,1,15));

        RecalculateCustomerTotals();

        var statuses = new[] { "Open","Investigating","Pending","Closed","Closed" };
        _claims = Enumerable.Range(1, 210).Select(i => new Claim { Id=i, ClaimNumber=$"CLM-{10000+i}", PolicyNumber=_policies[(i*7)%_policies.Count].PolicyNumber, Status=statuses[i%statuses.Length], Amount=900m+((i*1847)%52000), LossDate=new DateOnly(2026, ((i+4)%9)+1, ((i*5)%27)+1) }).ToList();
        _claims[0] = new Claim { Id=1, ClaimNumber="CLM-10482", PolicyNumber="POL-48392", Status="Investigating", Amount=42850m, LossDate=new DateOnly(2026,9,4) };
    }

    public Task<IReadOnlyList<Policy>> GetPoliciesAsync()=>Task.FromResult<IReadOnlyList<Policy>>(_policies);
    public Task<IReadOnlyList<Claim>> GetClaimsAsync()=>Task.FromResult<IReadOnlyList<Claim>>(_claims.OrderByDescending(c=>c.LossDate).ToList());
    public Task<IReadOnlyList<Customer>> GetCustomersAsync()=>Task.FromResult<IReadOnlyList<Customer>>(_customers.OrderBy(c=>c.Name).ToList());
    public Task<DashboardSummary> GetSummaryAsync(){var premium=_policies.Where(p=>p.Active).Sum(p=>p.Premium)*12m;var activeClaims=_claims.Where(c=>!c.IsArchived).ToList();var losses=activeClaims.Sum(c=>c.Amount);return Task.FromResult(new DashboardSummary(_policies.Count(p=>p.Active),activeClaims.Count(c=>c.Status is "Open" or "Investigating"),premium,activeClaims.Where(c=>c.Status!="Closed").Sum(c=>c.Amount),premium==0?0:losses/premium));}

    public Task<Claim> AddClaimAsync(Claim claim){claim.Id=_claims.Count==0?1:_claims.Max(c=>c.Id)+1;claim.LastUpdatedAt=DateTimeOffset.UtcNow;_claims.Add(claim);return Task.FromResult(claim);}
    public Task<Claim> UpdateClaimAsync(Claim claim){var existing=_claims.First(c=>c.Id==claim.Id);existing.PolicyNumber=claim.PolicyNumber;existing.Status=claim.Status;existing.Amount=claim.Amount;existing.LossDate=claim.LossDate;existing.LastUpdatedAt=DateTimeOffset.UtcNow;return Task.FromResult(existing);}
    public Task<Claim> ArchiveClaimAsync(int id){var claim=_claims.First(c=>c.Id==id);claim.IsArchived=true;claim.ArchivedAt=DateTimeOffset.UtcNow;claim.LastUpdatedAt=DateTimeOffset.UtcNow;return Task.FromResult(claim);}
    public Task<Claim> RestoreClaimAsync(int id){var claim=_claims.First(c=>c.Id==id);claim.IsArchived=false;claim.ArchivedAt=null;claim.LastUpdatedAt=DateTimeOffset.UtcNow;return Task.FromResult(claim);}

    public Task<Policy> UpdatePolicyAsync(Policy policy){var index=_policies.FindIndex(p=>p.Id==policy.Id);if(index<0)throw new InvalidOperationException("Policy not found.");_policies[index]=policy;RecalculateCustomerTotals();return Task.FromResult(policy);}

    public Task<Customer> AddCustomerAsync(Customer customer){customer.Id=_customers.Count==0?1:_customers.Max(c=>c.Id)+1;customer.LastUpdatedAt=DateTimeOffset.UtcNow;_customers.Add(customer);return Task.FromResult(customer);}
    public Task<Customer> UpdateCustomerAsync(Customer customer){var existing=_customers.First(c=>c.Id==customer.Id);var oldName=existing.Name;existing.Name=customer.Name;existing.Email=customer.Email;existing.State=customer.State;existing.LastUpdatedAt=DateTimeOffset.UtcNow;for(var i=0;i<_policies.Count;i++)if(_policies[i].CustomerName==oldName)_policies[i]=_policies[i] with { CustomerName=existing.Name };RecalculateCustomerTotals();return Task.FromResult(existing);}
    public Task<Customer> ArchiveCustomerAsync(int id){var customer=_customers.First(c=>c.Id==id);customer.IsArchived=true;customer.ArchivedAt=DateTimeOffset.UtcNow;customer.LastUpdatedAt=DateTimeOffset.UtcNow;return Task.FromResult(customer);}
    public Task<Customer> RestoreCustomerAsync(int id){var customer=_customers.First(c=>c.Id==id);customer.IsArchived=false;customer.ArchivedAt=null;customer.LastUpdatedAt=DateTimeOffset.UtcNow;return Task.FromResult(customer);}

    private void RecalculateCustomerTotals(){foreach(var c in _customers){var owned=_policies.Where(p=>p.CustomerName==c.Name).ToList();c.PolicyCount=owned.Count;c.TotalPremium=owned.Sum(p=>p.Premium)*12m;}}
}
