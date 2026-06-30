# cCoder.Workflow

`cCoder.Workflow` contains the Workflow domain for the cCoder platform.

## Functionality

The repository provides the Workflow domain packages and standalone hosts used by cCoder applications.

- Workflow web API
  Exposes OData endpoints for flow definitions, flow instance data, workflow events, execution, metadata discovery, SignalR workflow progress, and `/Health` through `AddWorkflowWeb` and `StartWorkflowWeb`.
- Workflow activities
  Provides reusable activities for API calls, DMS operations, templating, flow control, transformations, and workflow composition.
- Workflow engine
  Lives in the `src/Apps/Workflow` Functions app and executes workflow instances handed off by the hosted-services app.
- Workflow hosted-services host
  Runs background workflow event receivers, scheduled-task handlers, queued workflow handoff, and `/Health` through `AddWorkflowHostedServices` and `StartWorkflowHostedServices`.

## Contents

- `src/cCoder.Workflow`
  The main workflow library package published to NuGet.
- `src/cCoder.Workflow.Activities`
  Shared workflow activities package published from the same repository.
- `src/Workflow.Web`
  The standalone API web host for the Workflow domain.
- `src/Workflow.HostedServices`
  The standalone hosted-services app for background workflow execution.
- `src/Apps/Workflow`
  The Azure Functions app that hosts the workflow execution engine.
- `src/cCoder.Workflow.Tests`
  Unit tests for the domain.
- `src/Workflow.AcceptanceTests`
  Acceptance tests for the standalone app hosts, including cross-app execution through Web, Hosted Services, and Workflow.

## Build

```powershell
dotnet build src/cCoder.Workflow.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Workflow.sln -v minimal --no-build
```

## Local Configuration

The standalone hosts read local secrets from environment variables rather than committed config.

Before running `src/Workflow.Web` or `src/Workflow.HostedServices`, set:

- `ConnectionStrings__Core`
- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`
- `Settings__sslPort`
- `Services__HostedServices`
- `Services__Workflow`

`Services__HostedServices` should point to the hosted-services HTTP base URL, for example `http://localhost:5060`.
`Services__Workflow` should point to the Functions execution endpoint root, for example `http://localhost:7071/api/`.
`Settings__sslPort` should match the HTTPS port used by `Workflow.Web`, for example `7157`.

The committed `appsettings.json` files keep these values blank so user or machine environment variables can supply them during local development.

The acceptance tests can also read environment connection strings:

- `CCODER_ACCEPTANCE_CORE_CONNECTION_STRING`
- `CCODER_ACCEPTANCE_SSO_CONNECTION_STRING`

The test fixture creates suffixed databases from those connection strings and drops them when the suite completes.

## Run Locally

Run the API host:

```powershell
dotnet run --project src/Workflow.Web/Workflow.Web.csproj -c Release --launch-profile https
```

Once the host is running, verify readiness with:

```powershell
Invoke-RestMethod https://localhost:7157/Health
```

Run the hosted-services host:

```powershell
dotnet run --project src/Workflow.HostedServices/Workflow.HostedServices.csproj -c Release --launch-profile https
```

Once the hosted-services host is running, verify readiness with:

```powershell
Invoke-RestMethod https://localhost:7158/Health
```

Run the Workflow Functions host:

```powershell
func start --script-root src/Apps/Workflow --port 7071
```

Once the Functions host is running, verify readiness with:

```powershell
Invoke-RestMethod http://localhost:7071/api/Health
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
