// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http.Features;


namespace Apps.Shared.Hosting;

public sealed class NoOpSessionFeature : ISessionFeature
{
    public ISession Session { get; set; } = NoOpSession.Instance;
}