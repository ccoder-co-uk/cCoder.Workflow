param(
    [Parameter(Mandatory)]
    [string] $Version
)

$ErrorActionPreference = "Stop"

function Invoke-BuildWave {
    param(
        [Parameter(Mandatory)]
        [string[]] $Projects
    )

    $processes = foreach ($project in $Projects) {
        Start-Process dotnet -NoNewWindow -PassThru -ArgumentList @(
            "build",
            $project,
            "-c", "Release",
            "--no-restore",
            "-m",
            "/p:BuildProjectReferences=false",
            "/p:BuildInParallel=true",
            "/p:UseSharedCompilation=false",
            "/p:Version=$Version"
        )
    }

    $processes | Wait-Process
    $failedProcesses = @($processes | Where-Object ExitCode -ne 0)

    if ($failedProcesses.Count -ne 0) {
        throw "$($failedProcesses.Count) build(s) failed."
    }
}

Invoke-BuildWave -Projects @(
    "src/cCoder.Workflow.Activities/cCoder.Workflow.Activities.csproj"
)

Invoke-BuildWave -Projects @(
    "src/cCoder.Workflow.Engine/cCoder.Workflow.Engine.csproj",
    "src/cCoder.Workflow/cCoder.Workflow.csproj"
)

Invoke-BuildWave -Projects @(
    "src/Apps/Workflow/Workflow.csproj",
    "src/Workflow.HostedServices/Workflow.HostedServices.csproj",
    "src/Workflow.Web/Workflow.Web.csproj",
    "src/cCoder.Workflow.Activities.Tests/cCoder.Workflow.Activities.Tests.csproj",
    "src/cCoder.Workflow.Engine.Tests/cCoder.Workflow.Engine.Tests.csproj",
    "src/cCoder.Workflow.Tests/cCoder.Workflow.Tests.csproj"
)

Invoke-BuildWave -Projects @(
    "src/cCoder.Workflow.IntegrationTests/cCoder.Workflow.IntegrationTests.csproj"
)

Invoke-BuildWave -Projects @(
    "src/Workflow.AcceptanceTests/Workflow.AcceptanceTests.csproj"
)

Invoke-BuildWave -Projects @(
    "src/Workflow.HostedServices.AcceptanceTests/Workflow.HostedServices.AcceptanceTests.csproj"
)

Invoke-BuildWave -Projects @(
    "src/Workflow.Web.AcceptanceTests/Workflow.Web.AcceptanceTests.csproj"
)
