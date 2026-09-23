// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Foundations;
using FluentAssertions;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed class WorkflowRequestBodyServiceTests
{
    [Fact]
    public async Task ReadTextAsync_WhenStreamContainsText_ReturnsExactText()
    {
        const string expected = "workflow request body";
        byte[] bytes = Encoding.UTF8.GetBytes(s: expected);
        using MemoryStream stream = new(buffer: bytes);
        WorkflowRequestBodyService service = new(
            streamBroker: new StreamBroker());

        string actual = await service.ReadTextAsync(stream: stream);

        actual.Should().Be(expected: expected);
    }
}