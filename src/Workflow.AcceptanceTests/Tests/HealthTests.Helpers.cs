// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.AcceptanceTests.Infrastructure;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class HealthTests
{
    private static TestHttpRequestData CreateRequest()
    {
        return new TestHttpRequestData();
    }

    private static string ReadBody(TestHttpResponseData response)
    {
        return response.ReadBody();
    }
}