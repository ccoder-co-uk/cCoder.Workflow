// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class WorkflowHubTests
{
    [Fact]
    public async Task ShouldReturnNonErrorResponseForNegotiate()
    {
        // Given

        // When
        int actualStatusCode = await NegotiateAsync();

        // Then
        actualStatusCode.Should()
            .NotBe(unexpected: (int)HttpStatusCode.NotFound);
        actualStatusCode.Should()
            .NotBe(unexpected: (int)HttpStatusCode.InternalServerError);
    }
}