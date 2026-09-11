# .NET Insurance Dashboard

A hands-on learning project for modern **C#**, **Blazor WebAssembly**, **ASP.NET Core APIs**, **dependency injection**, **LINQ**, **async/await**, reusable components, forms, services, repositories, and API architecture.

The sample insurance data is fictional. This project is not affiliated with National General, Allstate, or any other insurer.

## Architecture

```text
Blazor WebAssembly UI
        |
        v
ASP.NET Core API
        |
        v
Services + Dependency Injection
        |
        v
Repositories
        |
        v
In-memory sample data
```

The Blazor client also has **demo mode**, allowing it to run on GitHub Pages without the ASP.NET Core backend.

## Projects

- `InsuranceDashboard.Client` — Blazor WebAssembly frontend
- `InsuranceDashboard.Api` — ASP.NET Core Minimal API
- `InsuranceDashboard.Shared` — shared records/models

## Requirements

Install the .NET 8 SDK.

Check:

```bash
dotnet --version
```

## Run the full stack locally

### 1. Start the API

```bash
dotnet run --project InsuranceDashboard.Api
```

The API runs at:

```text
http://localhost:5159
```

### 2. Start the Blazor client

In another terminal:

```bash
dotnet run --project InsuranceDashboard.Client
```

The development configuration is already set to use the real API.

## GitHub Pages

GitHub Pages cannot run an ASP.NET Core server. The Pages deployment uses the client's in-browser demo repository instead.

After pushing this project to GitHub:

1. Open the repository.
2. Go to **Settings → Pages**.
3. Under **Build and deployment → Source**, choose **GitHub Actions**.
4. Push to `main`.
5. The workflow in `.github/workflows/deploy-pages.yml` deploys the Blazor frontend.

Expected URL:

```text
https://YOUR-USERNAME.github.io/dotnet-insurance-dashboard/
```

## Learning path using this dashboard

1. **C# Models + LINQ**  
   `InsuranceDashboard.Shared/Models`

2. **Interfaces + Dependency Injection**  
   `InsuranceDashboard.Api/Repositories` and `Program.cs`

3. **Blazor Components**  
   `InsuranceDashboard.Client/Components`

4. **Blazor State + Events**  
   `Pages/Claims.razor`

5. **Forms + Validation**  
   `Pages/Claims.razor`

6. **ASP.NET Core APIs**  
   `InsuranceDashboard.Api/Program.cs`

7. **Async/Await**  
   `Client/Services` and API repositories

8. **Architecture**  
   Follow the request from page → service → API → repository → response

## Useful commands

Build everything:

```bash
dotnet build InsuranceDashboard.sln
```

Publish the client:

```bash
dotnet publish InsuranceDashboard.Client -c Release
```

## Suggested first lesson

Open:

```text
InsuranceDashboard.Client/Pages/Claims.razor
```

Then trace:

```text
Claims.razor
→ IInsuranceDataService
→ ApiInsuranceDataService
→ GET /api/claims
→ IClaimsRepository
→ ClaimsRepository
```

That single path covers Blazor, interfaces, DI, async/await, HTTP, APIs, and repositories.
