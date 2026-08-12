// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowMigrationAggregationServiceTests
{
    [Fact]
    public void ShouldExportEmptyPackageForUnknownPackageName()
    {
        // Given
        Mock<IWorkflowMigrationServiceProviderBroker> brokerMock =
            new(behavior: MockBehavior.Strict);

        WorkflowMigrationAggregationService service =
            new(serviceProviderBroker: brokerMock.Object);

        // When
        WorkflowPackage package = service.ExportPackage(
            appId: 1,
            packageName: "Unknown");

        // Then
        package.Name
            .Should()
            .Be(expected: "Unknown");

        package.Items
            .Should()
            .BeEmpty();

        brokerMock.VerifyNoOtherCalls();
    }
}