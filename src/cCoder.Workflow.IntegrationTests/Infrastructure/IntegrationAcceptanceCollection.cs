// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace Web.AcceptanceTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationAcceptanceCollection
    : ICollectionFixture<IntegrationAcceptanceFixture>
{
    public const string Name = "Integration acceptance";
}