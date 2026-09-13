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
app.MapGet("/api/claims", async (IClaimsRepository repository) => Results.Ok(await repository.GetAllAsync()));
app.MapGet("/api/claims/{id:int}", async (int id, IClaimsRepository repository) => { var item = await repository.GetByIdAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPost("/api/claims", async (Claim claim, IClaimsRepository repository) => { var created = await repository.AddAsync(claim); return Results.Created($"/api/claims/{created.Id}", created); });

app.MapGet("/api/customers", async (ICustomerRepository repository) => Results.Ok(await repository.GetAllAsync()));
app.MapGet("/api/customers/{id:int}", async (int id, ICustomerRepository repository) => { var item = await repository.GetByIdAsync(id); return item is null ? Results.NotFound() : Results.Ok(item); });
app.MapPost("/api/customers", async (Customer customer, ICustomerRepository repository) => { var created = await repository.AddAsync(customer); return Results.Created($"/api/customers/{created.Id}", created); });
app.MapPut("/api/customers/{id:int}", async (int id, Customer customer, ICustomerRepository repository) => { var updated = await repository.UpdateAsync(id, customer); return updated is null ? Results.NotFound() : Results.Ok(updated); });
app.MapDelete("/api/customers/{id:int}", async (int id, ICustomerRepository repository) => await repository.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/api/summary", async (IPolicyRepository policyRepository, IClaimsRepository claimsRepository) =>
{
    var policies = await policyRepository.GetAllAsync(); var claims = await claimsRepository.GetAllAsync();
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
