# cCoder.Workflow

`cCoder.Workflow` contains the Workflow domain for the cCoder platform.

## Local Configuration

Each app binds its committed `appsettings.json` directly into its root
configuration object. Leave secret values empty and define these user- or
machine-level environment variables:

- `Data__ConnectionString`
- `Workflow__ConnectionString`
- `Security__ConnectionString`
- `Security__DecryptionKey`
- `Eventing__ServiceBus__ConnectionString` when Service Bus eventing is selected

Restart Visual Studio after changing environment variables, select the required
startup projects, and press F5. No conversion, `.env` file, or startup script is
required.

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
dotnet build src/cCoder.Workflow.slnx -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Workflow.slnx -v minimal --no-build
```

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
