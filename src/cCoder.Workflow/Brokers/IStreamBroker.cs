// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace cCoder.Workflow.Brokers;

internal interface IStreamBroker
{
    ValueTask<string> ReadTextAsync(Stream stream);
}