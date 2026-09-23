// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.Workflow.Brokers;

internal sealed class StreamBroker : IStreamBroker
{
    public async ValueTask<string> ReadTextAsync(Stream stream)
    {
        using MemoryStream content = new();
        await stream.CopyToAsync(destination: content);

        return Encoding.UTF8.GetString(bytes: content.ToArray());
    }
}