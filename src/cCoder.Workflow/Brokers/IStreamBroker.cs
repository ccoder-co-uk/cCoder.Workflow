// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IStreamBroker
{
    ValueTask<string> ReadTextAsync(Stream stream);
}