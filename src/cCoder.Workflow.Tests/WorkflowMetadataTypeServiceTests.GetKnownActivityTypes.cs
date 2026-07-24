// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities.Sftp;
using cCoder.Workflow.Activities.Activities.Templating;
using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Xunit;


namespace cCoder.Workflow.Tests;

public partial class WorkflowMetadataTypeServiceTests
{
    [Fact]
    public void ShouldReturnKnownActivityGroupsOnGetKnownActivityTypes()
    {
        // Given

        // When
        var result = service.GetKnownActivityTypes();

        // Then
        result.Select(selector: set => set.Name)
            .Should()
            .Equal(elements:
            [
                "ApiActivity",
                "DMSActivity",
                "LogActivity",
                "SftpActivity",
                "TemplatingActivity",
                "TransformationActivity",
                "Workflow"
            ]);
    }

    [Fact]
    public void ShouldReturnSharedWorkflowMetadataOnGetSharedMetadata()
    {
        // Given

        // When
        var result = service.GetSharedMetadata();

        // Then
        result.Name.Should()
            .Be(expected: "Workflow");

        result.Types.Select(selector: type => type.Name)
            .Should()
            .Contain(expected: [
                nameof(Start),
                nameof(PageBuilder),
                nameof(SendEmailActivity),
                nameof(SftpGetFolderContentsActivity),
                nameof(SftpMoveFilesToFolderActivity),
                nameof(SftpUploadTextFilesToFolderActivity),
                nameof(Flow),
                nameof(WorkflowLogEntry),
            ]);

        result.Types.Select(selector: type => type.Name)
            .Should()
            .NotContain(unexpected: [
                "SftpFetchActivity",
                "SftpMoveActivity",
                "SftpCreateBinaryFilesActivity",
                "SftpCreateTextFilesActivity",
                "SftpDeleteActivity",
            ]);
    }

    [Fact]
    public void ShouldReturnKnownSystemTypesOnGetKnownSystemTypes()
    {
        // Given

        // When
        var result = service.GetKnownSystemTypes();

        // Then
        result.Should()
            .ContainSingle();

        result[0].Name.Should()
            .Be(expected: "System");

        result[0].Types.Select(selector: type => type.Name)
            .Should()
            .Contain(expected:
            [
                nameof(Int32),
                nameof(String),
                nameof(DateTime),
                nameof(TimeSpan)
            ]);
    }
}