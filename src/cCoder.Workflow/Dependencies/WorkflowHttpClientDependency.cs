// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;

namespace cCoder.Workflow.Dependencies;

internal sealed class WorkflowHttpClientDependency : IDisposable
{
    private readonly HttpClient client = new(handler: new HttpClientHandler
    {
        AutomaticDecompression =
            DecompressionMethods.GZip | DecompressionMethods.Deflate
    });

    public async ValueTask<string> PostTextAsync(
        string apiRoot,
        TimeSpan timeout,
        string requestUri,
        string content)
    {
        client.BaseAddress = new Uri(apiRoot);
        client.Timeout = timeout;

        using HttpResponseMessage response = await client.PostAsync(
            requestUri: requestUri,
            content: new StringContent(
                content: content,
                encoding: Encoding.UTF8,
                mediaType: "text/plain"));

        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose() =>
        client.Dispose();
}