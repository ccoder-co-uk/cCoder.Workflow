// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class RuntimeDependencyTests
{
    [Fact]
    public void WorkflowAssembly_WhenRuntimeDependenciesAreInspected_ReferencesContractsAndNotAnalyzer()
    {
        // Given
        string[] referencedAssemblies = typeof(WorkflowHubService)
            .Assembly
            .GetReferencedAssemblies()
            .Select(selector: assemblyName => assemblyName.Name)
            .ToArray();

        // When
        bool referencesContracts = referencedAssemblies.Contains(
            value: "cCoder.CodeAnalysis.Contracts");

        bool referencesAnalyzer = referencedAssemblies.Contains(
            value: "cCoder.CodeAnalysis");

        // Then
        Assert.True(condition: referencesContracts);
        Assert.False(condition: referencesAnalyzer);
    }
}