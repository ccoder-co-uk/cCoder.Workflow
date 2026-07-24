// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Headers;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Activities.Api;

public abstract class ApiActivity : Activity
{
    public string AuthType { get; set; } = "bearer";
    public string AuthToken { get; set; }
    public string BaseUrl { get; set; }

    public override Task ExecuteInternal(IWorkflowContext context)
    {
        BaseUrl ??= context.Variables["Api"] as string;
        AuthToken ??= context.Variables["AuthToken"] as string;
        return base.ExecuteInternal(context);
    }

    protected HttpClient GetHttpClient()
    {
        HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback = CertChainValidator.ValidateCertChain
        }).WithBaseUri(BaseUrl);

        httpClient.Timeout = TimeSpan.FromSeconds(200.0);

        if (AuthToken != null)
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthType, AuthToken);

        return httpClient;
    }
}

/// <summary>
/// Base type for all Http interactions with workflow
/// </summary>
/// <typeparam name="T">result type expected</typeparam>
public abstract class ApiActivity<T> : ApiActivity
{
    [JsonIgnore]
    public int BatchSize { get; set; } = 500;

    public string Query { get; set; }

    [IgnoreWhenFlowComplete]
    [JsonIgnore]
    public T Result { get; set; }
}