param(
    [switch]$CollectCoverage
)

$ErrorActionPreference = "Stop"

$testProjects = @(
    "src/cCoder.Workflow.Activities.Tests/cCoder.Workflow.Activities.Tests.csproj",
    "src/cCoder.Workflow.Engine.Tests/cCoder.Workflow.Engine.Tests.csproj",
    "src/cCoder.Workflow.Tests/cCoder.Workflow.Tests.csproj",
    "src/cCoder.Workflow.IntegrationTests/cCoder.Workflow.IntegrationTests.csproj",
    "src/Workflow.AcceptanceTests/Workflow.AcceptanceTests.csproj",
    "src/Workflow.HostedServices.AcceptanceTests/Workflow.HostedServices.AcceptanceTests.csproj",
    "src/Workflow.Web.AcceptanceTests/Workflow.Web.AcceptanceTests.csproj"
)

New-Item -ItemType Directory -Path "artifacts/test-results" -Force | Out-Null

$processes = foreach ($project in $testProjects) {
    $resultName = [IO.Path]::GetFileNameWithoutExtension($project)
    $arguments = @(
        "test",
        $project,
        "-c", "Release",
        "--no-build",
        "--no-restore",
        "--logger", "trx;LogFileName=$resultName.trx",
        "--results-directory", "artifacts/test-results"
    )

    if ($CollectCoverage) {
        $arguments += @(
            '--collect:"XPlat Code Coverage"',
            '--settings', 'coverage.runsettings'
        )
    }

    Start-Process dotnet -NoNewWindow -PassThru -ArgumentList $arguments
}

$processes | Wait-Process
$failedProcesses = @($processes | Where-Object ExitCode -ne 0)

if ($failedProcesses.Count -ne 0) {
    throw "$($failedProcesses.Count) test project(s) failed."
}
