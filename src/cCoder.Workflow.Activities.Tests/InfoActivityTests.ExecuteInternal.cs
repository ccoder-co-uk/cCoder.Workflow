// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Xunit;

namespace cCoder.Workflow.Activities.Tests;

public sealed partial class InfoActivityTests
{
    [Fact]
    public async Task ExecuteInternal_LogsConfiguredMessage()
    {
        // Given
        InfoActivity activity = CreateInfoActivity();
        TestWorkflowContext context = new();

        // When
        await activity.ExecuteInternal(context: context);

        // Then
        context.ExecutionLog.Should().Contain(predicate: entry =>
            entry.Level == WorkflowLogLevel.Info.ToString()
            && entry.Message == "info:: hello");
    }
}