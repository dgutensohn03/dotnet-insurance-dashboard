using InsuranceDashboard.Api.Repositories;
using InsuranceDashboard.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins("http://localhost:5198", "https://localhost:7198")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IClaimsRepository, ClaimsRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/policies", async (IPolicyRepository repository) =>
{
    var policies = await repository.GetAllAsync();
    return Results.Ok(policies);
});

app.MapGet("/api/policies/{id:int}", async (int id, IPolicyRepository repository) =>
{
    var policy = await repository.GetByIdAsync(id);
    return policy is null ? Results.NotFound() : Results.Ok(policy);
});

app.MapGet("/api/claims", async (IClaimsRepository repository) =>
{
    var claims = await repository.GetAllAsync();
    return Results.Ok(claims);
});

app.MapGet("/api/claims/{id:int}", async (int id, IClaimsRepository repository) =>
{
    var claim = await repository.GetByIdAsync(id);
    return claim is null ? Results.NotFound() : Results.Ok(claim);
});

app.MapPost("/api/claims", async (Claim claim, IClaimsRepository repository) =>
{
    var created = await repository.AddAsync(claim);
    return Results.Created($"/api/claims/{created.Id}", created);
});

app.MapGet("/api/customers", async (ICustomerRepository repository) =>
{
    var customers = await repository.GetAllAsync();
    return Results.Ok(customers);
});

app.MapGet("/api/summary", async (
    IPolicyRepository policyRepository,
    IClaimsRepository claimsRepository) =>
{
    var policies = await policyRepository.GetAllAsync();
    var claims = await claimsRepository.GetAllAsync();

    var summary = new DashboardSummary(
        ActivePolicies: policies.Count(p => p.Active),
        OpenClaims: claims.Count(c => c.Status is "Open" or "Investigating"),
        WrittenPremium: policies.Where(p => p.Active).Sum(p => p.Premium),
        OpenClaimExposure: claims.Where(c => c.Status != "Closed").Sum(c => c.Amount));

    return Results.Ok(summary);
});

app.Run();
