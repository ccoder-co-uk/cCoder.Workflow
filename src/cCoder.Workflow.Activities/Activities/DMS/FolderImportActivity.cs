// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Headers;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.DMS;

public class FolderImportActivity : DMSActivity
{
    public string RemoteApiUrl { get; set; }
    public string RemoteAuthToken { get; set; }
    public string RemotePath { get; set; }

    public override async Task ExecuteAsync()
    {
        await base.ExecuteAsync();
        using HttpClient remoteApi = GetRemoteHttpClient();
        using HttpClient localApi = GetHttpClient();

        Log(level:WorkflowLogLevel.Info, message:$"Downloading from {RemoteApiUrl}DMS/{RemotePath}");
        HttpResponseMessage response = await remoteApi.GetAsync(requestUri:$"DMS/{RemotePath}", completionOption:HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            Log(level:WorkflowLogLevel.Warning, message:$"Source {RemoteApiUrl}DMS/{RemotePath} returned nothing downloadable:\n" +
                $"HTTP Status: {response.StatusCode}:\n{await response.Content.ReadAsStringAsync()}");

            State = ActivityState.Skipped;
            return;
        }

        using Stream remoteFolderStream = await response.Content.ReadAsStreamAsync();
        MemoryStream memoryStream = new();
        await remoteFolderStream.CopyToAsync(destination:memoryStream);
        memoryStream.Position = 0;

        Log(level:WorkflowLogLevel.Info, message:$"Importing to {localApi.BaseAddress}DMS/{Path}");
        response = await localApi.PostAsync(requestUri:$"DMS/{Path}?unpack=true&ignoreArchiveRoot=true", content:new StreamContent(memoryStream));

        if (!response.IsSuccessStatusCode)
        {
            Log(level:WorkflowLogLevel.Error, message:$"Failed to upload to {localApi.BaseAddress}DMS/{Path} due to error:\n" +
                $"HTTP Status: {response.StatusCode}:\n{await response.Content.ReadAsStringAsync()}");

            State = ActivityState.Failed;
        }
    }

    protected HttpClient GetRemoteHttpClient()
    {
        HttpClient result = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
            .WithBaseUri(baseUriString:RemoteApiUrl);

        if (RemoteAuthToken != null)
            result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", RemoteAuthToken);

        return result;
    }
}