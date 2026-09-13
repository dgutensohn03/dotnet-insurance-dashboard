using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private static readonly List<Customer> Customers =
    [
        new(1, "Avery Johnson", "avery@example.com", "CO", 1, 1500m),
        new(2, "Morgan Lee", "morgan@example.com", "TX", 1, 2880m),
        new(3, "Jordan Smith", "jordan@example.com", "AZ", 1, 2100m),
        new(4, "Casey Brown", "casey@example.com", "CO", 1, 3720m),
        new(5, "Riley Garcia", "riley@example.com", "NM", 1, 2376m),
        new(6, "Taylor Wilson", "taylor@example.com", "CO", 1, 504m)
    ];

    public Task<IReadOnlyList<Customer>> GetAllAsync() => Task.FromResult<IReadOnlyList<Customer>>(Customers.OrderBy(c => c.Name).ToList());
    public Task<Customer?> GetByIdAsync(int id) => Task.FromResult(Customers.FirstOrDefault(c => c.Id == id));

    public Task<Customer> AddAsync(Customer customer)
    {
        customer.Id = Customers.Count == 0 ? 1 : Customers.Max(c => c.Id) + 1;
        customer.LastUpdatedAt = DateTimeOffset.UtcNow;
        Customers.Add(customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> UpdateAsync(int id, Customer customer)
    {
        var existing = Customers.FirstOrDefault(c => c.Id == id);
        if (existing is null) return Task.FromResult<Customer?>(null);
        existing.Name = customer.Name; existing.Email = customer.Email; existing.State = customer.State; existing.LastUpdatedAt = DateTimeOffset.UtcNow;
        return Task.FromResult<Customer?>(existing);
    }

    public Task<Customer?> ArchiveAsync(int id)
    {
        var existing = Customers.FirstOrDefault(c => c.Id == id);
        if (existing is null) return Task.FromResult<Customer?>(null);
        existing.IsArchived = true; existing.ArchivedAt = DateTimeOffset.UtcNow; existing.LastUpdatedAt = DateTimeOffset.UtcNow;
        return Task.FromResult<Customer?>(existing);
    }

    public Task<Customer?> RestoreAsync(int id)
    {
        var existing = Customers.FirstOrDefault(c => c.Id == id);
        if (existing is null) return Task.FromResult<Customer?>(null);
        existing.IsArchived = false; existing.ArchivedAt = null; existing.LastUpdatedAt = DateTimeOffset.UtcNow;
        return Task.FromResult<Customer?>(existing);
    }
}
