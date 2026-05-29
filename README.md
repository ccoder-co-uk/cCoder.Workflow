# cCoder.Workflow

`cCoder.Workflow` contains the Workflow domain for the cCoder platform.

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
