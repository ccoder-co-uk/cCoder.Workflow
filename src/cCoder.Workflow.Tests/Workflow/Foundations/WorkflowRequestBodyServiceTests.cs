// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Foundations;
using FluentAssertions;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowRequestBodyServiceTests
{
    [Fact]
    public async Task ReadTextAsync_WhenStreamContainsText_ReturnsExactText()
    {
        // Given
        const string expected = "workflow request body";

        byte[] bytes = Encoding.UTF8.GetBytes(s: expected);
        using MemoryStream stream = new(buffer: bytes);

        WorkflowRequestBodyService service = new(
            streamBroker: new StreamBroker());

        // When
        string actual = await service.ReadTextAsync(stream: stream);

        // Then
        actual
            .Should()
            .Be(expected: expected);
    }
}