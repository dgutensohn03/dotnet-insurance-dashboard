using InsuranceDashboard.Shared.Models;

namespace InsuranceDashboard.Api.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private static readonly List<Customer> Customers =
    [
        new(1, "Avery Johnson", "avery@example.com", "CO", 1, 125m),
        new(2, "Morgan Lee", "morgan@example.com", "TX", 1, 240m),
        new(3, "Jordan Smith", "jordan@example.com", "AZ", 1, 175m),
        new(4, "Casey Brown", "casey@example.com", "CO", 1, 310m),
        new(5, "Riley Garcia", "riley@example.com", "NM", 1, 198m),
        new(6, "Taylor Wilson", "taylor@example.com", "CO", 1, 42m)
    ];

    public Task<IReadOnlyList<Customer>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Customer>>(Customers);
}
