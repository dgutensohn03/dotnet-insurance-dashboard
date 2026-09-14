using InsuranceDashboard.Api.Repositories;
using InsuranceDashboard.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:5198", "https://localhost:7198").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IClaimsRepository, ClaimsRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
var app = builder.Build();
app.UseCors();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/policies", async (IPolicyRepository repository) => Results.Ok(await repository.GetAllAsync()));
app.MapGet("/api/policies/{id:int}", async (int id, IPolicyRepository repository) => { var item = await repository.GetByIdAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPut("/api/policies/{id:int}", async (int id, Policy policy, IPolicyRepository repository) => { var updated = await repository.UpdateAsync(id, policy); return updated is null ? Results.NotFound() : Results.Ok(updated); });

app.MapGet("/api/claims", async (IClaimsRepository repository) => Results.Ok(await repository.GetAllAsync()));
app.MapGet("/api/claims/{id:int}", async (int id, IClaimsRepository repository) => { var item = await repository.GetByIdAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPost("/api/claims", async (Claim claim, IClaimsRepository repository) => { var created = await repository.AddAsync(claim); return Results.Created($"/api/claims/{created.Id}", created); });
app.MapPut("/api/claims/{id:int}", async (int id, Claim claim, IClaimsRepository repository) => { var updated = await repository.UpdateAsync(id, claim); return updated is null ? Results.NotFound() : Results.Ok(updated); });
app.MapPost("/api/claims/{id:int}/archive", async (int id, IClaimsRepository repository) => { var item = await repository.ArchiveAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPost("/api/claims/{id:int}/restore", async (int id, IClaimsRepository repository) => { var item = await repository.RestoreAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });

app.MapGet("/api/customers", async (ICustomerRepository repository) => Results.Ok(await repository.GetAllAsync()));
app.MapGet("/api/customers/{id:int}", async (int id, ICustomerRepository repository) => { var item = await repository.GetByIdAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPost("/api/customers", async (Customer customer, ICustomerRepository repository) => { var created = await repository.AddAsync(customer); return Results.Created($"/api/customers/{created.Id}", created); });
app.MapPut("/api/customers/{id:int}", async (int id, Customer customer, ICustomerRepository repository) => { var updated = await repository.UpdateAsync(id, customer); return updated is null ? Results.NotFound() : Results.Ok(updated); });
app.MapPost("/api/customers/{id:int}/archive", async (int id, ICustomerRepository repository) => { var item = await repository.ArchiveAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPost("/api/customers/{id:int}/restore", async (int id, ICustomerRepository repository) => { var item = await repository.RestoreAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });

app.MapGet("/api/summary", async (IPolicyRepository policyRepository, IClaimsRepository claimsRepository) =>
{
    var policies = await policyRepository.GetAllAsync(); var claims = (await claimsRepository.GetAllAsync()).Where(c => !c.IsArchived).ToList();
    var writtenPremium = policies.Where(p => p.Active).Sum(p => p.Premium) * 12m;
    var incurredLosses = claims.Sum(c => c.Amount);
    var summary = new DashboardSummary(
        policies.Count(p => p.Active),
        claims.Count(c => c.Status is "Open" or "Investigating"),
        writtenPremium,
        claims.Where(c => c.Status != "Closed").Sum(c => c.Amount),
        writtenPremium == 0 ? 0 : incurredLosses / writtenPremium);
    return Results.Ok(summary);
});
app.Run();
