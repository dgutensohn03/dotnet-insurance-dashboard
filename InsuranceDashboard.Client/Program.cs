using InsuranceDashboard.Client;
using InsuranceDashboard.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

var useDemoData = builder.Configuration.GetValue("UseDemoData", true);

if (useDemoData)
{
    builder.Services.AddScoped<IInsuranceDataService, DemoInsuranceDataService>();
}
else
{
    builder.Services.AddScoped<IInsuranceDataService, ApiInsuranceDataService>();
}

await builder.Build().RunAsync();
