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
        var result = service.GetKnownActivityTypes();

        result.Select(set => set.Name)
            .Should()
            .Equal(
                "ApiActivity",
                "DMSActivity",
                "LogActivity",
                "SftpActivity",
                "TemplatingActivity",
                "TransformationActivity",
                "Workflow"
            );
    }

    [Fact]
    public void ShouldReturnSharedWorkflowMetadataOnGetSharedMetadata()
    {
        var result = service.GetSharedMetadata();

        result.Name.Should().Be("Workflow");
        result.Types.Select(type => type.Name)
            .Should()
            .Contain([
                nameof(Start),
                nameof(PageBuilder),
                nameof(SendEmailActivity),
                nameof(SftpFetchActivity),
                nameof(Flow),
                nameof(WorkflowLogEntry),
            ]);
    }

    [Fact]
    public void ShouldReturnKnownSystemTypesOnGetKnownSystemTypes()
    {
        var result = service.GetKnownSystemTypes();

        result.Should().ContainSingle();
        result[0].Name.Should().Be("System");
        result[0].Types.Select(type => type.Name)
            .Should()
            .Contain(nameof(Int32), nameof(String), nameof(DateTime), nameof(TimeSpan));
    }
}

