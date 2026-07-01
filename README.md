# cCoder.Workflow

`cCoder.Workflow` contains the Workflow domain for the cCoder platform.

## Functionality

The repository provides the Workflow domain packages and standalone hosts used by cCoder applications.

- Workflow web API
  Exposes OData endpoints for flow definitions, flow instance data, workflow events, execution, metadata discovery, SignalR workflow progress, `/Health`, and a simple root Workflow tester UI through `AddWorkflowWeb` and `StartWorkflowWeb`.
- Workflow activities
  Provides reusable activities for API calls, DMS operations, templating, flow control, transformations, and workflow composition.
- Workflow engine
  Lives in the `src/cCoder.Workflow.Engine` package. It exposes `IFlowRunner`, script execution services, and `AddWorkflowEngine()` for apps that need to execute workflow instances.
- Workflow hosted-services host
  Runs background workflow event receivers, scheduled-task handlers, queued workflow handoff, instance maintenance, queue-state repair, `/Health`, and a root hosted-services report through `AddWorkflowHostedServices` and `StartWorkflowHostedServices`. It uses the default `cCoder.Eventing.Http` `/Api/Eventing` dispatcher.

## Contents

- `src/cCoder.Workflow`
  The main workflow library package published to NuGet.
- `src/cCoder.Workflow.Activities`
  Shared workflow activities package published from the same repository.
- `src/cCoder.Workflow.Engine`
  Workflow execution engine package consumed by the Functions app.
- `src/Workflow.Web`
  The standalone API web host for the Workflow domain.
- `src/Workflow.HostedServices`
  The standalone hosted-services app for background workflow execution.
- `src/Apps/Workflow`
  The Azure Functions app that hosts thin HTTP/function triggers and delegates execution to `cCoder.Workflow.Engine`.
- `src/cCoder.Workflow.Tests`
  Unit tests for the domain.
- `src/cCoder.Workflow.Activities.Tests`
  Unit tests for workflow activity behaviour.
- `src/cCoder.Workflow.Engine.Tests`
  Unit tests for the workflow engine public exposures and orchestration wiring.
- `src/Workflow.AcceptanceTests`
  Acceptance tests for the Workflow Functions app.
- `src/Workflow.Web.AcceptanceTests`
  Acceptance tests for the standalone Workflow web API host.
- `src/Workflow.HostedServices.AcceptanceTests`
  Acceptance tests for the standalone Workflow hosted-services host.
- `src/cCoder.Workflow.IntegrationTests`
  Cross-process tests for Web, Hosted Services, and Workflow execution scenarios.

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
- `Eventing__Http__MaxConcurrency`

`Services__HostedServices` should point to the hosted-services HTTP base URL, for example `http://localhost:5060`.
`Services:Workflow` is committed in app config with the local Functions default `http://localhost:7071/api/`. Override it with `Services__Workflow` only when the Functions app is hosted elsewhere.
`Settings__sslPort` should match the HTTPS port used by `Workflow.Web`, for example `7157`.
`Eventing__Http__MaxConcurrency` controls the shared HTTP event dispatcher concurrency and can be `1` for local verification.

The committed `appsettings.json` files keep these values blank so user or machine environment variables can supply them during local development.

Before running `src/Apps/Workflow`, set `AzureWebJobsStorage` to a valid Functions storage connection string. For local development this is usually `UseDevelopmentStorage=true` with Azurite running. The HTTP functions can respond without it, but the Functions host reports storage health as unhealthy until the setting is present and reachable.

Visual Studio installations may include Azurite under `Common7\IDE\Extensions\Microsoft\Azure Storage Emulator`. Add that folder to `PATH` if `azurite --version` does not resolve in a terminal.

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

Open `https://localhost:7157/` to use the lightweight Workflow tester UI for flow management, definition editing, and execution handoff.

Run the hosted-services host:

```powershell
dotnet run --project src/Workflow.HostedServices/Workflow.HostedServices.csproj -c Release --launch-profile https
```

Once the hosted-services host is running, verify readiness with:

```powershell
Invoke-RestMethod https://localhost:7158/Health
```

Open `https://localhost:7158/` to see the hosted services and event listeners registered by the app.

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
- `cCoder.Workflow.Engine`

## Repository Alignment Notes

`Workflow.HostedServices` intentionally uses the default `cCoder.Eventing.Http` controller and receive-provider pipeline. The older custom HTTP event controller override pattern should not be copied here.

Follow-up outside this repository: `ccoder.Core` still has the same HTTP event controller override pattern and should be cleaned up to align with the default `cCoder.Eventing.Http` dispatcher model.

## Publishing

GitHub Actions is configured to publish the workflow library packages using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.Workflow`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.
