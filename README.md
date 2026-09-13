# InsureOps

A production-inspired insurance operations dashboard built with **Blazor WebAssembly**, **ASP.NET Core**, and **C#/.NET 8**.

**Live demo:** https://dgutensohn03.github.io/dotnet-insurance-dashboard/

InsureOps brings policy, customer, claims, and portfolio data into one operations workspace. The project is intentionally fictional and unbranded; it is designed to demonstrate full-stack architecture, domain-aware UX decisions, maintainable component boundaries, testing, and delivery practices rather than imitate any real insurer.

## What this project demonstrates

- Blazor WebAssembly component architecture and state management
- ASP.NET Core Minimal APIs and dependency injection
- Shared typed domain models across client and server
- LINQ-based portfolio filtering, summaries, and prioritization
- Customer create/edit workflows with validation
- Recoverable **archive / restore** lifecycles instead of destructive deletion
- Claim intake, prioritization, detail review, and lifecycle management
- Responsive modal and drawer interaction patterns
- Service and repository boundaries that keep UI, HTTP, and persistence concerns separate
- xUnit coverage for portfolio calculations and lifecycle behavior
- GitHub Actions quality gate: tests must pass before Pages deployment
- A static GitHub Pages demo mode plus a full local client/API architecture

## Product areas

### Overview
Portfolio KPIs, policy mix, claims workload, recent activity, high-exposure attention items, and a simplified loss-ratio signal.

### Policies
Search and filter coverage by customer, product, and status. Summary cards surface annualized active premium, active policy count, and average monthly premium.

### Claims
Search and prioritize by status, severity, and lifecycle state. Claim intake uses a focused modal, while a detail drawer preserves table context. Claims are archived and restored rather than destructively deleted so operational history remains recoverable.

### Customers
Search customer relationships, create and edit records, inspect portfolio value, and manage Active / Archived / All lifecycle views. Archiving removes a customer from normal active workflows without erasing retained history.

### Analytics
Lightweight operational analytics built from the same typed portfolio data: product distribution, exposure by status, severity bands, average claim size, and portfolio rates.

## Architecture

```text
Blazor WebAssembly UI
        |
        v
IInsuranceDataService
     /      \
    /        \
Demo mode   HTTP client
(GitHub      |
 Pages)      v
        ASP.NET Core API
               |
               v
          Repositories
               |
               v
         Domain data
```

The client depends on `IInsuranceDataService`, not a concrete transport. On GitHub Pages, `DemoInsuranceDataService` provides deterministic fictional data in-browser. In full-stack local development, `ApiInsuranceDataService` calls the ASP.NET Core API. This keeps hosting constraints from leaking into the UI layer.

## Domain decision: archive instead of delete

A basic CRUD implementation would permanently delete customers or claims. That is deliberately not the default here.

The application models lifecycle state with `IsArchived`, `ArchivedAt`, and `LastUpdatedAt`. Archive and restore operations are exposed through the same service and API boundaries as other domain actions. Active operational summaries exclude archived claims, while archived records remain visible through lifecycle filters and can be restored.

This is a small example of a larger engineering principle: **the domain should determine the interaction model, not the CRUD verbs available in a framework.**

## Testing and delivery

`InsuranceDashboard.Tests` includes xUnit coverage for:

- the stable case-study claim (`CLM-10482`)
- portfolio summary calculations
- customer create/update/archive/restore behavior
- claim archive/restore recovery

The GitHub Actions workflow runs the test project before publishing the Blazor client. A failing test or compile error blocks deployment.

## Repository structure

```text
InsuranceDashboard.Client   Blazor WebAssembly UI
InsuranceDashboard.Api      ASP.NET Core Minimal API
InsuranceDashboard.Shared   Shared domain models
InsuranceDashboard.Tests    xUnit tests
```

## Run locally

Requires the .NET 8 SDK.

```bash
dotnet run --project InsuranceDashboard.Api
```

Then, in another terminal:

```bash
dotnet run --project InsuranceDashboard.Client
```

The development client is configured to use the API. The API defaults to `http://localhost:5159`.

Run the tests:

```bash
dotnet test InsuranceDashboard.Tests/InsuranceDashboard.Tests.csproj
```

Build the solution:

```bash
dotnet build InsuranceDashboard.sln
```

## GitHub Pages demo

GitHub Pages cannot host an ASP.NET Core process, so the deployed site uses the browser-based demo service while retaining the same client interface and UX. Archive/create/edit changes in the Pages demo are session-only and reset on reload; local full-stack mode exercises the HTTP/API path.

The deployment workflow:

```text
push to main
→ restore/build through dotnet test
→ publish Blazor client
→ configure Pages base path
→ upload artifact
→ deploy
```

## Engineering tradeoffs

This project deliberately keeps the persistence layer in-memory so the focus stays on application architecture and interaction design. A production implementation would typically add durable storage, authentication and authorization, stronger relationship constraints, telemetry, audit persistence, concurrency handling, DTO/entity separation, integration/E2E coverage, and domain-specific financial calculations.

The loss-ratio-style metric is intentionally a portfolio demo signal rather than an actuarial calculation; a production implementation would align incurred losses and earned premium over the same reporting period.

## Case study

The companion portfolio case study walks through the same application from user interaction to Blazor state, service boundaries, HTTP, ASP.NET Core, dependency injection, repositories, testing, deployment, and production tradeoffs.

---

Built as a fictional portfolio project. No real customer information is used, and the project is not affiliated with National General, Allstate, or any other insurer.
