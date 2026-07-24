// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.AcceptanceTests.Infrastructure;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class HealthTests
{
    private readonly Health function = new();

    private static TestHttpRequestData CreateRequest() =>
        new();

    private static string ReadBody(TestHttpResponseData response) =>
        response.ReadBody();
}