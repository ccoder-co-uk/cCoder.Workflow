# cCoder.Workflow

`cCoder.Workflow` contains the Workflow domain for the cCoder platform.

## Functionality

The repository provides the Workflow domain packages and standalone host used by cCoder applications.

- Workflow API
  Exposes OData endpoints for flow definitions, flow instance data, workflow events, execution, and metadata discovery.
- Workflow activities
  Provides reusable activities for API calls, DMS operations, templating, flow control, transformations, and workflow composition.
- Workflow engine
  Manages queued workflow instances, scheduled execution handoff, workflow event subscriptions, and background execution orchestration.
- Workflow web host
  Runs the standalone Workflow API, SignalR workflow hub, Swagger documentation, and `/Health` readiness endpoint.

## Contents

- `src/cCoder.Workflow`
  The main workflow library package published to NuGet.
- `src/cCoder.Workflow.Activities`
  Shared workflow activities package published from the same repository.
- `src/Workflow.Web`
  The standalone web host for the Workflow domain.
- `src/cCoder.Workflow.Tests`
  Unit tests for the domain.
- `src/Workflow.AcceptanceTests`
  Acceptance tests for the standalone host.

## Build

```powershell
dotnet build src/cCoder.Workflow.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Workflow.sln -v minimal --no-build
```

## Local Configuration

The standalone web host reads local secrets from environment variables rather than committed config.

Before running `src/Workflow.Web`, set:

- `ConnectionStrings__Core`
- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

The acceptance tests can also read environment connection strings:

- `CCODER_ACCEPTANCE_CORE_CONNECTION_STRING`
- `CCODER_ACCEPTANCE_SSO_CONNECTION_STRING`

The test fixture creates suffixed databases from those connection strings and drops them when the suite completes.

## Run Locally

```powershell
dotnet run --project src/Workflow.Web/Workflow.Web.csproj -c Release --launch-profile https
```

Once the host is running, verify readiness with:

```powershell
Invoke-RestMethod https://localhost:7157/Health
```

## Packages

The NuGet packages produced by this repository are:

- `cCoder.Workflow`
- `cCoder.Workflow.Activities`

## Publishing

GitHub Actions is configured to publish the workflow library packages using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.Workflow`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.
